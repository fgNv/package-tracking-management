
module Actions

open Railroad
open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.ServerErrors
open Sentences
open System
open JsonParse
open Application

let private language = Language.PtBr

let inline private executeCommand deserializeCommand handleCommand (request : HttpRequest) =   
    try 
        let result = request.rawForm |> deserializeCommand >>= handleCommand
        match result with
            | Railroad.Success i -> NO_CONTENT
            | Railroad.Error errorContent -> 
                    let (title, errors) = GetErrorContentTup language errorContent
                    let responseContent = { Title = title
                                            Errors = errors |> Seq.toList } : JsonParse.InvalidDataResponse
                    BAD_REQUEST (JsonParse.QueryResult.serializeObj responseContent)
    with
        | ex -> 
            System.Console.WriteLine()
            let responseContent = { Title = ex.Message
                                    Errors = [ex.Message; ex.StackTrace] } : JsonParse.InvalidDataResponse
            INTERNAL_ERROR(JsonParse.QueryResult.serializeObj responseContent)

let fromQueryString transformation defaultValue (request : HttpRequest) paramName =
    match request.queryParam paramName with
        | Choice1Of2 p -> transformation p
        | choice2of2 -> defaultValue
let intFromQueryString defaultValue request paramName = 
    fromQueryString Int32.Parse defaultValue request paramName
let optionFromQueryString request paramName = 
    fromQueryString Some None request paramName
    
let getUserById id =
    let result = Application.User.GetById {Id = id}
    match result with  
        | Success user ->
            match user with
                | Some u -> OK (QueryResult.serializeObj u)
                | None -> NOT_FOUND(translate language Sentence.IdMustReferToAnExistingUser) 
        | Error(_) -> INTERNAL_ERROR (translate language Sentence.DatabaseFailure) 

let getPackageById id =
    let result = Application.Package.GetDetails {PackageId = id}
    match result with 
        | Success package ->
            match package with 
                | Some p -> OK (QueryResult.serializeObj p)
                | None -> 
                    NOT_FOUND (translate language Sentence.IdMustReferToExistingPackage)
        | Error (_) -> INTERNAL_ERROR (translate language Sentences.DatabaseFailure) 

let updatePackage ctx =
    let userId = Claims.getUserIdFromContext ctx    
    request(executeCommand (UpdatePackageCommand.deserialize userId) 
                            Package.Update)
let deletePackage id ctx  =
    let userId = Claims.getUserIdFromContext ctx  
    let cmd = {Id = id; UserId = userId} : Commands.Package.Delete.Command
    match Package.Delete cmd with | Success _ -> NO_CONTENT
                                    | Error content ->
                                        let (title, errors) = GetErrorContentTup language content 
                                        INTERNAL_ERROR(title) 

let grantPermission =
    request(executeCommand JsonParse.GrantPermission.deserialize User.GrantPermission)

let getPackages context =
    let request = context.request
    let page = intFromQueryString 1 request "page" 
    let itemsPerPage = intFromQueryString 20 request "itemsPerPage"
    let nameFilter = optionFromQueryString request "nameFilter" 
    let userId = Claims.getUserIdFromContext context  

    let query = { Page = page 
                  ItemsPerPage = itemsPerPage
                  CurrentUserId = userId
                  NameFilter = nameFilter } : Queries.Package.List.Query
    match Application.Package.GetList query with
        | Success r -> OK (QueryResult.serializeObj r)
        | Error content -> 
            let (title, errors) = GetErrorContentTup language content 
            INTERNAL_ERROR(title) 

let createPackage ctx =
    let userId = Claims.getUserIdFromContext ctx    
    request(executeCommand (CreatePackageCommand.deserialize userId) 
                            Package.Create)    

let createManualPoint ctx =
    let userId = Claims.getUserIdFromContext ctx    
    request(executeCommand (CreateManualPointCommand.deserialize userId)
                            Package.AddManualPoint)

let removeManualPoint ctx =
    let userId = Claims.getUserIdFromContext ctx    
    request(executeCommand (RemoveManualPoint.deserialize userId)
                            Package.RemoveManualPoint)

let updateUser ctx =
    let userId = Claims.getUserIdFromContext ctx    
    request(executeCommand (UpdateUserCommand.deserialize userId) User.Update)

let getUsers request =
    let page = intFromQueryString 1 request "page" 
    let itemsPerPage = intFromQueryString 20 request "itemsPerPage"
    let nameFilter = optionFromQueryString request "nameFilter" 
    let accessTypeFilter = optionFromQueryString request "accessTypeFilter" 
    let mappedAccessType = match accessTypeFilter with 
                                | Some x -> match x with 
                                            | v when v = "administrator" -> Some Models.AccessType.Administrator
                                            | v when v = "user" -> Some Models.AccessType.User
                                            | _ -> None
                                | None -> None
    let query = { Page = page 
                  ItemsPerPage = itemsPerPage
                  NameFilter = nameFilter
                  AccessTypeFilter = mappedAccessType } : Queries.User.List.Query
    match Application.User.GetList query with
        | Success r -> OK (QueryResult.serializeObj r)
        | Error content -> 
            let (title, errors) = GetErrorContentTup language content 
            INTERNAL_ERROR(title)

let deleteUser id ctx =
    let userId = Claims.getUserIdFromContext ctx  
    let cmd = {Id = id; CurrentUserId = userId} : Commands.User.Delete.Command
    match User.Delete cmd with | Success _ -> NO_CONTENT
                                | Error content -> 
                                    let (title, errors) = GetErrorContentTup language content 
                                    INTERNAL_ERROR(title)

let createUser ctx =
    let userId = Claims.getUserIdFromContext ctx                       
    request (executeCommand (CreateUserCommand.deserialize userId) 
                                User.Create)

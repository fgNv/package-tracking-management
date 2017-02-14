module Routes

open Railroad
open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.ServerErrors

let inline private executeCommand deserializeCommand handleCommand (request : HttpRequest) =   
    try 
        let result = request.rawForm |> deserializeCommand >>= handleCommand
        match result with
              | Success i -> OK "done"
              | Error (title, errors) -> 
                    BAD_REQUEST (title + " - errors: ")
    with
        | ex -> INTERNAL_ERROR(ex.Message)

open Suave.Operators
open Suave.Filters
open JsonParse
open Application
open Suave.ServerErrors
open System
open Suave.Writers
open Suave.CORS

module private Actions =
    let fromQueryString transformation defaultValue (request : HttpRequest) paramName =
        match request.queryParam paramName with
            | Choice1Of2 p -> transformation p
            | choice2of2 -> defaultValue
    let intFromQueryString = fromQueryString Int32.Parse
    let optionFromQueryString = fromQueryString Some None
    let getUserById id =
        let result = Application.User.GetById {Id = id}
        match result with  
            | Success user ->
                match user with
                    | Some u -> OK (QueryResult.serializeObj u)
                    | None -> NOT_FOUND(Sentences.Validation.IdMustReferToExistingUser) 
            | Error(_) -> INTERNAL_ERROR ("Sentences.Error.DatabaseFailure") 

    let getPackageById id =
        let result = Application.Package.GetDetails {PackageId = id}
        match result with 
            | Success package ->
                match package with 
                    | Some p -> OK (QueryResult.serializeObj p)
                    | None -> 
                       NOT_FOUND ("Sentences.Validation.IdMustReferToExistingPackage")
            | Error (_) -> INTERNAL_ERROR ("Sentences.Error.DatabaseFailure") 
    
    let updatePackage ctx =
        let userId = Claims.getUserIdFromContext ctx    
        request(executeCommand (UpdatePackageCommand.deserialize userId) 
                                Package.Update)
    let deletePackage id ctx  =
        let userId = Claims.getUserIdFromContext ctx  
        let cmd = {Id = id; UserId = userId} : Commands.Package.Delete.Command
        match Package.Delete cmd with | Success _ -> NO_CONTENT
                                      | Error(title, errs) -> INTERNAL_ERROR(title) 

    let getPackages request =
        let page = intFromQueryString 1 request "page" 
        let itemsPerPage = intFromQueryString 20 request "itemsPerPage"
        let nameFilter = optionFromQueryString request "nameFilter" 
        let query = { Page = page 
                      ItemsPerPage = itemsPerPage
                      NameFilter = nameFilter } : Queries.Package.List.Query
        match Application.Package.GetList query with
            | Success r -> OK (QueryResult.serializeObj r)
            | Error(e,_) -> INTERNAL_ERROR(e) 

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
        let query = { Page = page 
                      ItemsPerPage = itemsPerPage
                      NameFilter = nameFilter } : Queries.User.List.Query
        match Application.User.GetList query with
            | Success r -> OK (QueryResult.serializeObj r)
            | Error(e,_) -> INTERNAL_ERROR(e) 

    let deleteUser id ctx =
        let userId = Claims.getUserIdFromContext ctx  
        let cmd = {Id = id; CurrentUserId = userId} : Commands.User.Delete.Command
        match User.Delete cmd with | Success _ -> NO_CONTENT
                                   | Error(title, errs) -> INTERNAL_ERROR(title)

    let createUser ctx =
        let userId = Claims.getUserIdFromContext ctx                       
        request (executeCommand (CreateUserCommand.deserialize userId) 
                                 User.Create)

let apiRoutes =
    let protectResource = 
        ResourceProtection.protectResource [|Suave.Authentication.UserNameKey; Claims.UserIdKey|]
    
    let setCORSHeaders =
        setHeader  "Access-Control-Allow-Origin" "*"
        >=> setHeader "Access-Control-Allow-Headers" "content-type"

    let setJsonHeaders = 
        setHeader "Content-type" "application/json"

    let corsConfig = { defaultCORSConfig with allowedUris = InclusiveOption.All
                                              allowedMethods = InclusiveOption.Some [HttpMethod.DELETE; HttpMethod.GET; HttpMethod.POST; HttpMethod.PUT; HttpMethod.OPTIONS]  }
    
    let jsonEndpoints = 
        protectResource (
            choose [             
                 path "/package/point/manual" >=> 
                        choose [ POST >=> context(Actions.createManualPoint) ]
                        choose [ DELETE >=> context(Actions.removeManualPoint) ]
                 pathScan "/package/%s" 
                   (fun id -> 
                      let parsedId = System.Guid.Parse id
                      choose [ 
                        GET >=> Actions.getPackageById parsedId
                        DELETE >=> context (Actions.deletePackage parsedId) ])
                 path "/package" >=>
                     choose [ 
                        GET >=> request(Actions.getPackages)
                        POST >=> context (Actions.createPackage) 
                        PUT >=> context (Actions.updatePackage) ]
                 pathScan "/user/%s" 
                   (fun id -> 
                      let parsedId = System.Guid.Parse id
                      choose [ DELETE >=> context (Actions.deleteUser parsedId)
                               GET >=> Actions.getUserById parsedId ])
                 path "/user" >=>
                        choose [ POST >=> context (Actions.createUser)
                                 PUT >=> context (Actions.updateUser)
                                 GET >=> request(Actions.getUsers) ] 
        ] )
        
    choose [ OPTIONS >=> cors corsConfig
             GET >=> path "/" >=> Files.browseFileHome "index.html"
             path "/token" >=> context(fun ctx ->
                                    let emptyCtx = {ctx with userState = Map.empty}
                                    let middleware = 
                                            AuthorizationServer.authorizationServerMiddleware 
                                               User.ChallengeCredentials
                                               Claims.getCustomClaims
                                    (fun ignore -> middleware(emptyCtx)) )
             Files.browseHome
             jsonEndpoints >=> setCORSHeaders >=> setJsonHeaders   
             NOT_FOUND ("no resource matches this path")
          ]

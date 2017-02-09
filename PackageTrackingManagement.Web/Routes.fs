module Routes

open Railroad
open Suave
open Suave.Successful
open Suave.RequestErrors

let inline private executeCommand deserializeCommand handleCommand (request : HttpRequest) =    
    let result = request.rawForm |> deserializeCommand >>= handleCommand
    match result with
          | Success i -> OK "done"
          | Error (title, errors) -> BAD_REQUEST (title + " - errors")

open Suave.Operators
open Suave.Filters
open JsonParse
open Application
open Suave.ServerErrors
open System
open Suave.Writers
open Suave.CORS

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

    let fromQueryString transformation defaultValue (request : HttpRequest) paramName =
        match request.queryParam paramName with
            | Choice1Of2 p -> transformation p
            | choice2of2 -> defaultValue

    let intFromQueryString = fromQueryString Int32.Parse
    let optionFromQueryString = fromQueryString Some None

    let jsonEndpoints = 
        protectResource (
            choose [             
                 pathScan "/package/%s" 
                   (fun id -> 
                      let parsedId = System.Guid.Parse id
                      choose [ 
                        GET >=> (   
                            let result = Application.Package.GetDetails {PackageId = parsedId}

                            match result with 
                                | Success package ->
                                    match package with 
                                        | Some p -> OK (QueryResult.serializeObj p)
                                        | None -> 
                                           NOT_FOUND ("Sentences.Validation.IdMustReferToExistingPackage")
                                | Error (_,_) -> INTERNAL_ERROR ("Sentences.Error.DatabaseFailure") )
                        DELETE >=> context (fun ctx ->
                                               let userId = Claims.getUserIdFromContext ctx  
                                               let cmd = {Id = parsedId; UserId = userId} : Commands.Package.Delete.Command
                                               match Package.Delete cmd with
                                                | Success _ -> NO_CONTENT
                                                | Error(title, errs) -> INTERNAL_ERROR(title) ) ])
                 path "/package" >=>
                     choose [ 
                        GET >=> request (fun request ->    
                            let page = intFromQueryString 1 request "page" 
                            let itemsPerPage = intFromQueryString 20 request "itemsPerPage"
                            let nameFilter = optionFromQueryString request "nameFilter" 

                            let query = { Page = page 
                                          ItemsPerPage = itemsPerPage
                                          NameFilter = nameFilter } : Queries.Package.List.Query

                            match Application.Package.GetList query with
                                | Success r -> OK (QueryResult.serializeObj r)
                                | Error(e,_) -> INTERNAL_ERROR(e) )
                        POST >=> context (fun ctx ->
                            let userId = Claims.getUserIdFromContext ctx    
                            request(executeCommand (CreatePackageCommand.deserialize userId) 
                                                    Package.Create) ) 
                        PUT >=> context (fun ctx ->
                            let userId = Claims.getUserIdFromContext ctx    
                            request(executeCommand (UpdatePackageCommand.deserialize userId) 
                                                    Package.Update) ) ]                  
                 path "/package/point/manual" >=> 
                        choose [
                            POST >=> context(fun ctx ->
                                let userId = Claims.getUserIdFromContext ctx    
                                request(executeCommand (CreateManualPointCommand.deserialize userId)
                                                        Package.AddManualPoint )
                            ) ]
                 path "/user" >=>
                        choose [ POST >=> 
                                    context (fun ctx ->      
                                      let userId = Claims.getUserIdFromContext ctx                       
                                      request (executeCommand (CreateUserCommand.deserialize userId) 
                                                               User.Create) ) ] 
        ] )
        
    choose [ OPTIONS >=> cors corsConfig
             GET >=> path "/" >=> 
                Files.browseFileHome "index.html"
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

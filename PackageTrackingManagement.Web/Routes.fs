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

    let corsConfig = { defaultCORSConfig with allowedUris = InclusiveOption.All }
        
    choose [ OPTIONS >=> cors corsConfig  
             path "/token" >=> context(fun ctx ->
                                let emptyCtx = {ctx with userState = Map.empty}
                                let middleware = 
                                        AuthorizationServer.authorizationServerMiddleware 
                                           User.ChallengeCredentials
                                           Claims.getCustomClaims
                                (fun ignore -> middleware(emptyCtx)) )
             pathScan "/package/%s" (fun id ->
                    protectResource (
                        let parsedId = System.Guid.Parse id
                        let result = Application.Package.GetDetails {PackageId = parsedId}

                        match result with 
                            | Success package ->
                                match package with 
                                    | Some p -> OK (QueryResult.serializeObj p)
                                    | None -> 
                                        NOT_FOUND (Sentences.Validation.IdMustReferToExistingPackage)
                            | Error (_,_) -> INTERNAL_ERROR (Sentences.Error.DatabaseFailure) )
             )                           
             path "/package" >=>
                protectResource (
                 choose [ 
                    GET >=> request (fun request ->    
                        let page = match request.queryParam "page" with
                                        | Choice1Of2 p -> Int32.Parse(p)
                                        | choice2of2 -> 1

                        let itemsPerPage = match request.queryParam "itemsPerPage" with
                                           | Choice1Of2 p -> Int32.Parse(p)
                                           | choice2of2 -> 20

                        let nameFilter = match request.queryParam "nameFilter" with
                                           | Choice1Of2 n -> Some n
                                           | choice2of2 -> None

                        let query = { Page = page 
                                      ItemsPerPage = itemsPerPage
                                      NameFilter = nameFilter } : Queries.Package.List.Query

                        (match Application.Package.GetList query with
                            | Success r -> OK (QueryResult.serializeObj r)
                            | Error(e,_) -> INTERNAL_ERROR(e)  ) >=> setCORSHeaders >=> setJsonHeaders )
                    POST >=> context (fun ctx ->
                        let userId = Claims.getUserIdFromContext ctx    
                        request(executeCommand (CreatePackageCommand.deserialize userId) 
                                                Package.Create) ) ]
             )
             path "/package/point/manual" >=>                
                protectResource( 
                    choose [
                        POST >=> context(fun ctx ->
                            let userId = Claims.getUserIdFromContext ctx    
                            request(executeCommand (CreateManualPointCommand.deserialize userId)
                                                    Package.AddManualPoint )
                        )
             ])
             path "/user" >=> protectResource(
               choose [ POST >=> 
                        context (fun ctx ->      
                          let userId = Claims.getUserIdFromContext ctx                       
                          request (executeCommand (CreateUserCommand.deserialize userId) 
                                                   User.Create) ) ] )
             GET >=> NOT_FOUND "no resource matches the request" ]

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

let apiRoutes =
    let protectResource = 
        ResourceProtection.protectResource [|Suave.Authentication.UserNameKey; Claims.UserIdKey|]

    choose [ path "/token" >=> AuthorizationServer.authorizationServerMiddleware 
                               User.ChallengeCredentials
                               Claims.getCustomClaims
             protectResource( pathScan "/package/%s" (fun id ->
                    let parsedId = System.Guid.Parse id
                    let result = Application.Package.GetDetails {PackageId = parsedId}
                    match result with 
                        | Success package ->
                            match package with 
                                | Some p -> OK (QueryResult.serializeObj p)
                                | None -> 
                                    NOT_FOUND (Sentences.Validation.IdMustReferToExistingPackage)
                        | Error (_,_) -> INTERNAL_ERROR (Sentences.Error.DatabaseFailure)
             ) )
             protectResource(                 
                path "/package" >=> 
                 choose [ 
                    GET >=> context (fun ctx ->
                        let userId = Claims.getUserIdFromContext ctx    
                        request(executeCommand (CreatePackageCommand.deserialize userId) 
                                                Package.Create) ) 
                    POST >=> context (fun ctx ->
                        let userId = Claims.getUserIdFromContext ctx    
                        request(executeCommand (CreatePackageCommand.deserialize userId) 
                                                Package.Create) ) ]
             )
             protectResource( 
                path "/package/point/manual" >=>
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
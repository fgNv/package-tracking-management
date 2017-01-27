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

let apiRoutes =
    let protectResource = 
        ResourceProtection.protectResource [|Suave.Authentication.UserNameKey; Claims.UserIdKey|]

    choose [ path "/token" >=> AuthorizationServer.authorizationServerMiddleware 
                               User.ChallengeCredentials
                               Claims.getCustomClaims
             path "/user" >=> protectResource(
               choose [ POST >=> 
                        context(fun ctx ->      
                          let userId = Claims.getUserIdFromContext ctx                       
                          request (executeCommand (CreateUserCommand.deserialize userId) User.Create) ) ] )
             GET >=> NOT_FOUND "no resource matches the request" ]
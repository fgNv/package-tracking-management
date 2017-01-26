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

let apiRoutes() =
    choose [ (*path "/user" >=> 
               choose [ POST >=> request 
                          (executeCommand (CreateUserCommand.deserialize currentUserId) User.Create) ]             
              *)
             GET >=> NOT_FOUND "no resource matches the request" ]
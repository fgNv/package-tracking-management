module Claims

open Suave
open System
open Queries.ChallengeUserCredentials

let UserIdKey = "userId"

let inline getCustomClaims (user : QueryResult) =
    [(Suave.Authentication.UserNameKey, user.UserName); (UserIdKey, user.Id.ToString())]

let getUserIdFromContext (context : HttpContext) =
     unbox(context.userState.[UserIdKey]) |> Guid.Parse
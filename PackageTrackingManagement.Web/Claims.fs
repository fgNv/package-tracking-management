module Claims

open Suave
open System
open Queries.User.ChallengeUserCredentials
open System.Collections.Generic

let UserIdKey = "userId"    

let getClientData (user : QueryResult) =
    let dict = new Dictionary<string, string>()
    dict.Add("accessType", Models.mapClientRepresentation user.AccessType)
    dict.Add("name", user.Name)
    dict :> IDictionary<string,string>

let inline getCustomClaims (user : QueryResult) =
    [(Suave.Authentication.UserNameKey, user.UserName); (UserIdKey, user.Id.ToString())]

let getUserIdFromContext (context : HttpContext) =
     unbox(context.userState.[UserIdKey]) |> Guid.Parse
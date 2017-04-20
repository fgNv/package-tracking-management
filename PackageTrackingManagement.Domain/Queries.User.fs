module Queries.User

open Models
open Railroad
open System

module ChallengeUserCredentials =
    type Query = { UserName: string
                   Password: string }

        with member this.GetPassword() = this.Password

    type QueryResult = { UserName: string
                         Name: string
                         Email: string
                         Password: string
                         Id : Guid
                         AccessType: AccessType }

    let handle (getUserByUserName : string -> Result<QueryResult option>) 
               (query : Query) =
        match getUserByUserName query.UserName with
         | Success response ->            
            match response with 
                | Some u -> let matches = u.Password = Password.getEncryptedPassword query
                            if matches then
                                Success (u)
                            else
                                Error(TitleSentenceMessages(
                                        Sentences.CouldNotAuthenticate, [Sentences.InvalidCredentials]))
                | None -> Error(TitleSentenceMessages(
                                    Sentences.CouldNotAuthenticate, [Sentences.InvalidCredentials]))
         | Error _ ->
            Error (TitleSentenceMessages(
                    Sentences.ErrorValidatingCredentials, [Sentences.CouldNotAccessDatabase]))

module UserExists =
    type Query = {UserName : string}

    let handle getUserByUserName query =
        match getUserByUserName query.UserName with
            | Success user ->
                match user with
                    | Some u -> true
                    | None -> false
            | Error(_) -> false
        
open Chiron
open Chiron.Operators

module Get = 
    type Query = { Id : Guid }
    type User = { Name: string
                  Id : Guid
                  UserName : string
                  Email : string
                  AccessType : Models.AccessType }
      with static member ToJson(x : User) =
               Json.write "name" x.Name
            *> Json.write "userName" x.UserName
            *> Json.write "id" x.Id
            *> Json.write "email" x.Email
            *> Json.write "accessType" x.AccessType

    let handle getUserById (query : Query) : Result<User option> = 
        getUserById query.Id


module List =
    type Query = { ItemsPerPage : int; 
                   Page : int
                   NameFilter : string option
                   AccessTypeFilter : Models.AccessType option }
    
    type User = { Name: string
                  Id : Guid
                  Email : string
                  AccessType : Models.AccessType }
      with static member ToJson(x : User) =
               Json.write "name" x.Name
            *> Json.write "id" x.Id
            *> Json.write "email" x.Email
            *> Json.write "accessType" x.AccessType

    type QueryResult =  { Items : User list
                          Total : int }
        with static member ToJson(x : QueryResult) =
               Json.write "items" x.Items
            *> Json.write "total" x.Total

    let handle getUserList (query : Query) : Result<QueryResult> = 
        getUserList query

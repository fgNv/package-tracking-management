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
                                Error(Sentences.Error.CouldNotAuthenticate, 
                                      [Sentences.Error.InvalidCredentials])
                | None -> Error(Sentences.Error.CouldNotAuthenticate, [Sentences.Error.InvalidCredentials])
         | Error (title, errors) ->
            Error (Sentences.Error.ErrorValidatingCredentials, 
                  [Sentences.Error.CouldNotAccessDatabase])

module UserExists =
    type Query = {UserName : string}

    let handle getUserByUserName query =
        match getUserByUserName query.UserName with
            | Success user ->
                match user with
                    | Some u -> true
                    | None -> false
            | Error(_,_) -> false
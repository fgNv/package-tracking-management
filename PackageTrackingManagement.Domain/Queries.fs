module Queries

open Models
open Railroad

module ChallengeUserCredentials =
    type Query = { UserName: string
                   Password: string }
        with member this.GetPassword() = this.Password

    type Result = { UserName: string
                    Name: string
                    Email: string
                    Password: string
                    AccessType: AccessType }

    let handle (getUserByUserName : string -> Result<Result>) 
               (query : Query) =
        match getUserByUserName query.UserName with
         | Success response ->            
            let matches = response.Password = Password.getEncryptedPassword query
            Success (matches, response)
         | Error (title, errors) ->
            Error (Sentences.Error.ErrorValidatingCredentials, 
                  [Sentences.Error.CouldNotAccessDatabase])
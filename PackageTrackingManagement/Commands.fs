module Commands

open Models
open System
open Railroad

module Validation =
    let inline validate getErrors input =
        let errors = getErrors input

        match errors |> Seq.isEmpty with
                | true -> Result.Success input
                | false -> Result.Error (Sentences.Validation.InvalidData, errors)

module CreateUserCommand =
    type Command = { UserName: string
                     Email: string
                     Password: string
                     AccessType: AccessType
                     CreatorId: Guid }
        with member this.GetPassword() = this.Password
                     
    let private getErrors isCreatorAdministrator isEmailAvailable isUserNameAvailable 
                parameter =
            seq { if String.IsNullOrWhiteSpace parameter.UserName then
                     yield Sentences.Validation.UserNameIsRequired 
                  if String.IsNullOrWhiteSpace parameter.Password then
                     yield Sentences.Validation.PasswordIsRequired 
                  if not (isCreatorAdministrator parameter.CreatorId) then
                     yield Sentences.Validation.OnlyAdministratorsMayPerformThisAction
                  if not (isUserNameAvailable parameter.UserName) then
                     yield Sentences.Validation.ThisUserNameIsNotAvailable
                  if not (isEmailAvailable parameter.Email) then
                     yield Sentences.Validation.ThisEmailIsNotAvailable } 
                     
    let private assignEncryptedPassword (input : Command) =
        let encrypted = Models.Password.getEncryptedPassword input
        Success {input with Password = encrypted}

    let handle isCreatorAdministrator isEmailAvailable isUserNameAvailable 
               insertUser command =        
        command |> Validation.validate (getErrors isCreatorAdministrator isEmailAvailable 
                                                  isUserNameAvailable)
                >>= assignEncryptedPassword
                >>= insertUser

module UpdateUserCommand =
    type Command = { UserName: string
                     Email: string
                     AccessType: AccessType
                     Id : Guid
                     CurrentUserId: Guid }
                     
    let private getErrors isCreatorAdministrator isEmailAvailable isUserNameAvailable 
                userExists parameter =
            seq { if String.IsNullOrWhiteSpace parameter.UserName then
                     yield Sentences.Validation.UserNameIsRequired 
                  if not (isCreatorAdministrator parameter.CurrentUserId) &&
                     parameter.CurrentUserId <> parameter.Id then
                     yield Sentences.Validation.OnlyAdministratorsMayPerformThisAction
                  if not (isUserNameAvailable parameter.UserName) then
                     yield Sentences.Validation.ThisUserNameIsNotAvailable
                  if not (userExists parameter.Id) then
                     yield Sentences.Validation.IdMustReferToAnExistingUser                     
                  if not (isEmailAvailable parameter.Email) then
                     yield Sentences.Validation.ThisEmailIsNotAvailable } 
                     
    let handle isCreatorAdministrator isEmailAvailable isUserNameAvailable userExists
               updateUser command =        
        command |> Validation.validate (getErrors isCreatorAdministrator isEmailAvailable 
                                                  isUserNameAvailable userExists)
                >>= updateUser

module UpdateUserPasswordCommand =
    type Command = { Id : Guid
                     Password: String
                     CurrentUserId: Guid }
        with member this.GetPassword() = this.Password
                     
    let private getErrors userExists parameter =
            seq { if not (userExists parameter.Id) then
                     yield Sentences.Validation.IdMustReferToAnExistingUser
                  if String.IsNullOrWhiteSpace parameter.Password then
                     yield Sentences.Validation.PasswordIsRequired } 
    
    let private assignEncryptedPassword (input : Command) =
        let encrypted = Models.Password.getEncryptedPassword input
        Success {input with Password = encrypted}

    let handle userExists updateUserPassword command =        
        command |> Validation.validate (getErrors userExists)
                >>= assignEncryptedPassword
                >>= updateUserPassword
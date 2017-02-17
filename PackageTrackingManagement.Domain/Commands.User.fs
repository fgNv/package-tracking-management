module Commands.User

open Models
open System
open Railroad
open Commands.Global

module Create =
    let machineId = Guid.NewGuid()

    type Command = { UserName: string
                     Name: string
                     Email: string
                     Password: string
                     AccessType: AccessType
                     CreatorId: Guid }
        with member this.GetPassword() = this.Password
                     
    let private getErrors isCreatorAdministratorFun isEmailAvailableFun 
                          isUserNameAvailableFun  parameter =

            match isEmailAvailableFun parameter.Email,
                  isUserNameAvailableFun parameter.UserName,
                  isCreatorAdministratorFun parameter.CreatorId with
                | Success isEmailAvailable, Success isUserNameAvailable, 
                  Success isCreatorAdministrator ->
                    seq { if String.IsNullOrWhiteSpace parameter.UserName then
                             yield "Sentences.Validation.UserNameIsRequired" 
                          if String.IsNullOrWhiteSpace parameter.Password then
                             yield "Sentences.Validation.PasswordIsRequired"
                          if not isCreatorAdministrator && parameter.CreatorId <> machineId then
                             yield "Sentences.Validation.OnlyAdministratorsMayPerformThisAction"
                          if not isUserNameAvailable then
                             yield "Sentences.Validation.ThisUserNameIsNotAvailable"
                          if not isEmailAvailable  then
                             yield "Sentences.Validation.ThisEmailIsNotAvailable" } 
                | Error(_), _, _ 
                | _, Error(_), _ 
                | _, _, Error(_) -> seq { yield "Sentences.Error.DatabaseFailure" }
                     
    let private assignEncryptedPassword (input : Command) =
        Success {input with Password = Models.Password.getEncryptedPassword input}

    let handle isCreatorAdministrator isEmailAvailable isUserNameAvailable 
               insertUser command =        
        command |> Validation.validate (getErrors isCreatorAdministrator isEmailAvailable 
                                                  isUserNameAvailable)
                >>= assignEncryptedPassword
                >>= insertUser

module Update =
    type Command = { UserName: string
                     Email: string
                     Id : Guid
                     Name : String
                     AccessType : AccessType
                     CurrentUserId: Guid }
                     
    let private getErrors isCreatorAdministratorFun isEmailAvailableFun isUserNameAvailableFun 
                userExistsFun parameter =

            match isCreatorAdministratorFun parameter.CurrentUserId,
                  isEmailAvailableFun (parameter.Email,parameter.Id),
                  isUserNameAvailableFun (parameter.UserName,parameter.Id),
                  userExistsFun parameter.Id with
            | Success isCreatorAdministrator, Success isEmailAvailable,
              Success isUserNameAvailable, Success userExists ->
                seq { if String.IsNullOrWhiteSpace parameter.UserName then
                         yield  Sentences.Validation.UserNameIsRequired
                      if not isCreatorAdministrator && parameter.CurrentUserId <> parameter.Id then
                         yield "Sentences.Validation.OnlyAdministratorsMayPerformThisAction"
                      if not isUserNameAvailable then
                         yield "Sentences.Validation.ThisUserNameIsNotAvailable"
                      if not userExists then
                         yield Sentences.Validation.IdMustReferToAnExistingUser
                      if not isEmailAvailable then
                         yield "Sentences.Validation.ThisEmailIsNotAvailable" } 
            | Error(_), _, _, _ 
            | _, Error(_), _, _ 
            | _, _, Error(_), _ 
            | _, _, _, Error(_) -> seq { yield "Sentences.Error.DatabaseFailure" }
                     
    let handle isCreatorAdministrator isEmailAvailable isUserNameAvailable userExists
               updateUser command =        
        command |> Validation.validate (getErrors isCreatorAdministrator isEmailAvailable 
                                                  isUserNameAvailable userExists)
                >>= updateUser

module UpdatePassword =
    type Command = { Id : Guid
                     Password: String
                     CurrentUserId: Guid }
        with member this.GetPassword() = this.Password
                     
    let private getErrors userExistsFunc parameter =            
            match userExistsFunc parameter.Id with
                | Success userExists ->    
                    seq { if not userExists then
                             yield "Sentences.Validation.IdMustReferToAnExistingUser"
                          if String.IsNullOrWhiteSpace parameter.Password then
                             yield "Sentences.Validation.PasswordIsRequired" } 
                | Error (_) -> 
                    seq { yield "Sentences.Error.DatabaseFailure" }
    
    let private assignEncryptedPassword (input : Command) =
        Success {input with Password = Models.Password.getEncryptedPassword input}

    let handle userExists updateUserPassword command =        
        command |> Validation.validate (getErrors userExists)
                >>= assignEncryptedPassword
                >>= updateUserPassword

module Delete =
    type Command = {Id : Guid
                    CurrentUserId : Guid}

    let private getErrors userExistsFun isCurrentUserAdministratorFun parameter =
        match userExistsFun parameter.Id,
              isCurrentUserAdministratorFun parameter.CurrentUserId with
            | Success userExists, Success isCurrentUserAdministrator ->
                seq { if not userExists then
                         yield "Sentences.Validation.IdMustReferToAnExistingUser"
                      if not isCurrentUserAdministrator then
                         yield Sentences.Validation.OnlyAdministratorsMayPerformThisAction
                      if parameter.Id = parameter.CurrentUserId then
                         yield Sentences.Validation.UserMayNotDeleteHimself } 
            | Error (_), _
            | _, Error (_) -> seq { yield "Sentences.Error.DatabaseFailure" }
    
    let handle userExists isCurrentUserAdministrator deleteUser command =        
        command |> Validation.validate (getErrors userExists isCurrentUserAdministrator)
                >>= deleteUser

module GrantPermission =
    type Command = { UserId : Guid 
                     PackageId : Guid }

    let private getErrors userExistsFun isUserObserverFun packageExistsFun command =
        match userExistsFun command.UserId, isUserObserverFun command.UserId, 
              packageExistsFun command.PackageId with
            | Success userExists, Success isUserObserver, Success packageExists ->
                seq {
                    if not userExists then 
                        yield "Sentences.PtBrValidationSentences.IdMustReferToExistingUser"
                    if not isUserObserver then  
                        yield "Sentences.PtBrValidationSentences.UserMustBeObserver"
                    if not packageExists then
                        yield "Sentences.PtBrValidationSentences.IdMustReferToExistingPackage" }
            | Error(_), _ ,_ 
            | _, Error(_) ,_ 
            | _, _, Error(_) -> seq { yield "Sentences.Error.DatabaseFailure" }

    let handle userExists isUserObserver packageExists grantPermission command =
        command |> Validation.validate (getErrors userExists isUserObserver packageExists)
                >>= grantPermission

module RevokePermission =
    type Command = { UserId : Guid 
                     PackageId : Guid }
    let private getErrors permissionExistsFun command =
        match permissionExistsFun (command.UserId, command.PackageId) with
            | Success permissionExists ->
                seq {
                    if not permissionExists then 
                        yield "Sentences.PtBrValidationSentences.IdMustReferToAnExistingPermission" }
            | Error(_) -> seq { yield "Sentences.Error.DatabaseFailure" }

    let handle permissionExists revokePermission command =
        command |> Validation.validate (getErrors permissionExists)
                >>= revokePermission
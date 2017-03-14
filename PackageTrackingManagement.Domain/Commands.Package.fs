module Commands.Package

open Models
open Commands.Global
open System
open Railroad
open Sentences

module Create =
    type Command = {Name : string
                    Description : string option
                    CreatorId : Guid}
        
    let private getErrors isCreatorAdministratorFun parameter =
            match isCreatorAdministratorFun parameter.CreatorId with
                | Success isCreatorAdministrator ->
                    seq { if String.IsNullOrWhiteSpace parameter.Name then
                             yield translate Language.PtBr Sentence.UserNameIsRequired
                          if not isCreatorAdministrator then
                             yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle isCreatorAdministrator insertPackage command =        
        command |> Validation.validate (getErrors isCreatorAdministrator)
                >>= insertPackage

module Update =
    type Command = {Name : string
                    Description : string option
                    CreatorId : Guid
                    Id : Guid}
        
    let private getErrors isCreatorAdministratorFun packageExistsFun parameter =
            match isCreatorAdministratorFun parameter.CreatorId,
                  packageExistsFun parameter.Id with
                | Success isCreatorAdministrator,
                  Success packageExists ->
                    seq { if String.IsNullOrWhiteSpace parameter.Name then
                             yield translate Language.PtBr Sentence.UserNameIsRequired
                          if not packageExists then
                             yield translate Language.PtBr Sentence.IdMustReferToExistingPackage
                          if not isCreatorAdministrator then
                             yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle isCreatorAdministrator packageExists updatePackage command =        
        command |> Validation.validate (getErrors isCreatorAdministrator packageExists)
                >>= updatePackage

module Delete =
    type Command = {Id : Guid; UserId : Guid}
    
    let private getErrors packageExistsFun isUserAdministratorFun parameter =
            match packageExistsFun parameter.Id,
                  isUserAdministratorFun parameter.UserId with
                | Success packageExists, Success isUserAdministrator ->
                    seq { if not packageExists then
                             yield translate Language.PtBr Sentence.IdMustReferToExistingPackage
                          if not isUserAdministrator then
                             yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _ 
                | _, Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle packageExists isUserAdministrator deletePackage command =        
        command |> Validation.validate (getErrors packageExists isUserAdministrator)
                >>= deletePackage

module AddManualPoint =
    type Command = { PackageId : Guid
                     Latitude : double
                     Longitude : double
                     CreatorId : Guid }
    
    let private getErrors isCreatorAdministratorFun packageExistsFun parameter =
            match isCreatorAdministratorFun parameter.CreatorId,
                  packageExistsFun parameter.PackageId with
                | Success isCreatorAdministrator,
                  Success packageExists ->
                    seq { if not packageExists then
                            yield translate Language.PtBr Sentence.IdMustReferToExistingPackage
                          if parameter.Latitude < -90.0 || parameter.Latitude > 90.0 then
                            yield translate Language.PtBr Sentence.LatitudeMustBeBetweenMinusNinetyAndNinety
                          if parameter.Longitude < -180.0 || parameter.Longitude > 180.0 then
                            yield translate Language.PtBr Sentence.LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty
                          if not isCreatorAdministrator then
                            yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle isCreatorAdministrator packageExists insertManualPoint command =        
        command |> Validation.validate (getErrors isCreatorAdministrator packageExists)
                >>= insertManualPoint

module AddDevicePoint =
    type Command = { PackageId : Guid
                     Latitude : double
                     Longitude : double
                     DeviceId : Guid }
    
    let private getErrors deviceExistsFun packageExistsFun parameter =
            match deviceExistsFun parameter.DeviceId,
                  packageExistsFun parameter.PackageId with
                | Success isCreatorAdministrator,
                  Success packageExists ->
                    seq { if not packageExists then
                             yield translate Language.PtBr Sentence.IdMustReferToExistingPackage
                          if parameter.Latitude < -90.0 || parameter.Latitude > 90.0 then
                            yield translate Language.PtBr Sentence.LatitudeMustBeBetweenMinusNinetyAndNinety
                          if parameter.Longitude < -180.0 || parameter.Longitude > 180.0 then
                            yield translate Language.PtBr Sentence.LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty
                          if not isCreatorAdministrator then
                             yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle deviceExists packageExists insertDevicePoint command =        
        command |> Validation.validate (getErrors deviceExists packageExists)
                >>= insertDevicePoint

module RemoveManualPoint =
    type Command = { PointId : Guid
                     UserId : Guid  }

    let private getErrors manualPointExistsFun isUserAdministratorFun parameter =
            match manualPointExistsFun parameter.PointId,
                  isUserAdministratorFun parameter.UserId with
                | Success manualPointExits,
                  Success isUserAdministrator ->
                    seq { if not manualPointExits then
                            yield translate Language.PtBr Sentence.IdMustReferToExistingManualPoint
                          if not isUserAdministrator then
                            yield translate Language.PtBr Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield translate Language.PtBr Sentence.DatabaseFailure }

    let handle manualPointExists isUserAdministrator deleteManualPoint command =        
        command |> Validation.validate (getErrors manualPointExists isUserAdministrator)
                >>= deleteManualPoint
        
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
                             yield Sentence.UserNameIsRequired
                          if not isCreatorAdministrator then
                             yield Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_) -> seq { yield Sentence.DatabaseFailure }

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
                             yield Sentence.UserNameIsRequired
                          if not packageExists then
                             yield Sentence.IdMustReferToExistingPackage
                          if not isCreatorAdministrator then
                             yield Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield Sentence.DatabaseFailure }

    let handle isCreatorAdministrator packageExists updatePackage command =        
        command |> Validation.validate (getErrors isCreatorAdministrator packageExists)
                >>= updatePackage

module Delete =
    type Command = {Id : Guid; UserId : Guid}
      with member x.GetPackageId() = x.Id 
           member x.GetUserId() = x.UserId

    let private getErrors packageExistsFun isUserAdministratorFun parameter =
            match packageExistsFun parameter.Id,
                  isUserAdministratorFun parameter.UserId with
                | Success packageExists, Success isUserAdministrator ->
                    seq { if not packageExists then
                             yield Sentence.IdMustReferToExistingPackage
                          if not isUserAdministrator then
                             yield Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _ 
                | _, Error(_) -> seq { yield Sentence.DatabaseFailure }

    let handle packageExists isUserAdministrator deletePackage command =        
        command |> Validation.validate (getErrors packageExists isUserAdministrator)
                >>= deletePackage

module AddManualPoint =
    type Command = { PackageId : Guid
                     Latitude : double
                     Longitude : double
                     CreatorId : Guid }

    type IExternalValidations =
      abstract member IsCreatorAdministrator : Guid -> Async<Result<bool>>
      abstract member PackageExists : Guid -> Async<Result<bool>>
    
    let private getErrors (externalValidations: IExternalValidations) parameter =
            let externalValidationsResultsTask = async {
              let isCreatorAdministratorTask = externalValidations.IsCreatorAdministrator parameter.CreatorId
              let packageExistsTask = externalValidations.PackageExists parameter.PackageId
              let! isCreatorAdministrator = isCreatorAdministratorTask
              let! packageExists = packageExistsTask
              
              return seq { 
                  match isCreatorAdministrator, packageExists with 
                    | Success isCreatorAdministrator, Success packageExists ->
                      if isCreatorAdministrator then
                        yield Sentence.OnlyAdministratorsMayPerformThisAction
                      if not packageExists then
                        yield Sentence.IdMustReferToExistingPackage
                    | Error(_), _
                    | _, Error(_) -> yield Sentence.DatabaseFailure
              }
            } 
            
            let externalValidationsResults = externalValidationsResultsTask |> Async.RunSynchronously

            let internalValidationResults = 
                    seq { if parameter.Latitude < -90.0 || parameter.Latitude > 90.0 then
                            yield Sentence.LatitudeMustBeBetweenMinusNinetyAndNinety
                          if parameter.Longitude < -180.0 || parameter.Longitude > 180.0 then
                            yield Sentence.LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty}
            
            externalValidationsResults |> Seq.append internalValidationResults

    let handle externalValidations insertManualPoint command =        
        command |> Validation.validate (getErrors externalValidations)
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
                             yield Sentence.IdMustReferToExistingPackage
                          if parameter.Latitude < -90.0 || parameter.Latitude > 90.0 then
                            yield Sentence.LatitudeMustBeBetweenMinusNinetyAndNinety
                          if parameter.Longitude < -180.0 || parameter.Longitude > 180.0 then
                            yield Sentence.LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty
                          if not isCreatorAdministrator then
                             yield Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield Sentence.DatabaseFailure }

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
                            yield Sentence.IdMustReferToExistingManualPoint
                          if not isUserAdministrator then
                            yield Sentence.OnlyAdministratorsMayPerformThisAction } 
                | Error(_), _
                | _, Error(_) -> seq { yield Sentence.DatabaseFailure }

    let handle manualPointExists isUserAdministrator deleteManualPoint command =        
        command |> Validation.validate (getErrors manualPointExists isUserAdministrator)
                >>= deleteManualPoint
        
module PgSqlPackagePersistence

open PgSqlPersistence
open Commands
open System
open Models
open Queries.User.ChallengeUserCredentials
open NpgsqlTypes
open FSharp.Data
open FSharp.Data.Sql

let private optionToNullableString input =
    match input with
        | Some v -> v
        | None -> null

let getPackageList =
    handleDatabaseException
        (fun (query : Queries.Package.List.Query) ->
            let context = getContext()
            let packages =  context.Public.Package 
                            |> Seq.filter(fun p -> match query.NameFilter with 
                                                    | Some n -> p.Name = n
                                                    | None -> true )
                            |> Seq.skip((query.Page - 1) * query.ItemsPerPage)
                            |> Seq.take(query.ItemsPerPage)
                            |> Seq.map (fun p -> {  Name = p.Name
                                                    Id = p.Id
                                                    Description = Option.ofObj p.Description 
                                                    CreatedAt = p.CreatedAt
                                                    UpdatedAt = p.UpdatedAt
                                                 } : Queries.Package.List.Package)
                            |> Seq.toList
            let packagesTotalCount = context.Public.Package |> Seq.length
            { Items = packages; Total = packagesTotalCount} : Queries.Package.List.QueryResult )

let private mapPackageDetails (package : PgsqlAccess.dataContext.``public.packageEntity``) =
    { Id = package.Id
      Name = package.Name
      Description = Option.ofObj package.Description 
      CreatedAt = package.CreatedAt
      UpdatedAt = package.UpdatedAt
      ManualPoints = package.``public.manual_point by id`` |> 
                     Seq.map(fun mp -> { CreatedAt = mp.CreatedAt
                                         Latitude = mp.Coordinates.X
                                         Longitude = mp.Coordinates.Y 
                                       } : Queries.Package.Details.ManualPoint )
      DevicePoints = package.``public.device_point by id`` |> 
                     Seq.map(fun dp -> { CreatedAt = dp.CreatedAt
                                         Latitude = dp.Coordinates.X
                                         Longitude = dp.Coordinates.Y 
                                         DeviceId = dp.DeviceId
                                       } : Queries.Package.Details.DevicePoint )
    } : Queries.Package.Details.QueryResult

let getPackageDetails =
    handleDatabaseException
        (fun (q : Queries.Package.Details.Query) ->
            let context = getContext()            
            query { for p in context.Public.Package do
                    where (p.Id = q.PackageId)
                    select ( mapPackageDetails p ) } |> Seq.tryHead
        )    

let insertPackage =
    handleDatabaseException
        ( fun (cmd : Package.Create.Command) -> 
                     let context = getContext()
                     let newPackage = context.Public.Package.Create()   
                     newPackage.Id <- Guid.NewGuid()
                     newPackage.Name <- cmd.Name
                     newPackage.CreatedAt <- DateTime.Now
                     newPackage.UpdatedAt <- DateTime.Now
                     newPackage.Description <- optionToNullableString cmd.Description
                     newPackage.CreatorId <- cmd.CreatorId
                     context.SubmitUpdates() ) 

let updatePackage =
    handleDatabaseException
        ( fun (cmd : Package.Update.Command) -> 
                     let context = getContext()
                     let package = context.Public.Package |> 
                                    Seq.tryFind (fun g -> g.Id = cmd.Id)
                        
                     match package with
                     | Some p -> p.Description <- optionToNullableString cmd.Description                                 
                                 p.Name <- cmd.Name
                                 p.UpdatedAt <- DateTime.Now
                                 context.SubmitUpdates()
                     | None -> 
                        raise (new Exception(Sentences.Validation.IdMustReferToExistingPackage)) ) 

let deletePackage =
    handleDatabaseException 
        ( fun (cmd : Package.Delete.Command) -> 
                    let context = getContext()
                    let package = context.Public.Package |> 
                                    Seq.tryFind (fun g -> g.Id = cmd.Id)
                    match package with
                      | Some p -> p.Delete()
                                  context.SubmitUpdates()
                      | None -> 
                        raise (new Exception(Sentences.Validation.IdMustReferToExistingPackage)) ) 

let packageExists =
    handleDatabaseException 
        (fun id ->
            let context = getContext()
            context.Public.Package |> Seq.exists(fun p -> p.Id = id) )

let manualPointExists =
    handleDatabaseException 
        (fun id ->
            let context = getContext()
            context.Public.ManualPoint |> Seq.exists(fun p -> p.Id = id) )

let insertManualPoint =
    handleDatabaseException
        (fun (cmd : Package.AddManualPoint.Command) ->
            let context = getContext()
            let newPoint = context.Public.ManualPoint.Create()
            newPoint.Coordinates <- new NpgsqlPoint(cmd.Latitude, cmd.Longitude)
            newPoint.CreatorId <- cmd.CreatorId
            newPoint.CreatedAt <- DateTime.Now
            newPoint.Id <- Guid.NewGuid()
            newPoint.PackageId <- cmd.PackageId
            context.SubmitUpdates() )

let deleteManualPoint =
    handleDatabaseException
        (fun (cmd : Package.RemoveManualPoint.Command) ->
            let context = getContext()
            let point = context.Public.ManualPoint |> 
                           Seq.tryFind (fun m -> m.Id = cmd.PointId)
            match point with 
                | Some p -> p.Delete()
                            context.SubmitUpdates()
                | None -> raise (new Exception(Sentences.Validation.IdMustReferToExistingPoint)) )
            
let insertDevicePoint =
    handleDatabaseException
        (fun (cmd : Package.AddDevicePoint.Command) ->
            let context = getContext()
            let newPoint = context.Public.DevicePoint.Create()
            newPoint.CreatedAt <- DateTime.Now
            newPoint.Coordinates <- new NpgsqlPoint(cmd.Latitude, cmd.Longitude)
            newPoint.DeviceId <- cmd.DeviceId
            newPoint.Id <- Guid.NewGuid()
            newPoint.PackageId <- cmd.PackageId

            let package = context.Public.Package |> Seq.tryFind(fun p -> p.Id = cmd.PackageId)
            match package with | Some p -> p.UpdatedAt <- DateTime.Now 
                               | None -> 
                                  raise (new Exception(Sentences.Validation.IdMustReferToExistingPackage))

            context.SubmitUpdates() )
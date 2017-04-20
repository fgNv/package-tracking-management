module PgSqlUserPersistence

open PgSqlPersistence
open PgSqlProvider
open Commands
open System
open Models
open Queries.User.ChallengeUserCredentials
open NpgsqlTypes
open FSharp.Data
open FSharp.Data.Sql

let private serializeAccessType input =
    match input with
        | Administrator -> "admin"
        | User -> "user"

let private deserializeAccessType input =
    match input with 
        | "admin" -> Administrator
        | "user" -> User
        | _ -> raise (Exception("Sentences.Error.InvalidAccessType"))
        
let userExists =
    handleDatabaseException ( fun id -> let context = getContext()
                                        context.Public.User |> Seq.exists (fun g -> g.Id = id) ) 
                                        
let getUserList =
    handleDatabaseException ( 
        fun (query' : Queries.User.List.Query) ->
            let context = getContext()

            let nameFilter = match query'.NameFilter with | Some x -> x + "%" | None -> ""

            let accessTypeFilter = match query'.AccessTypeFilter with | Some x -> serializeAccessType x
                                                                      | None -> ""
            let qtyToSkip = (query'.Page - 1) * query'.ItemsPerPage

            let dbQuery = query {
                for u in context.Public.User  do
                where ( (nameFilter = "" || u.Name =% (nameFilter)) &&
                        (accessTypeFilter = "" || u.AccessType = accessTypeFilter) )
                skip qtyToSkip
                take (query'.ItemsPerPage)
                select ({  Name = u.Name
                           Id = u.Id
                           Email = u.Email
                           AccessType = deserializeAccessType u.AccessType
                        } : Queries.User.List.User)
            } 
            let users = dbQuery |> Seq.toList

            let packagesTotalCount = context.Public.Package |> Seq.length
            { Items = users; Total = packagesTotalCount} : Queries.User.List.QueryResult 
    ) 
let getUserById id =
    handleDatabaseException ( fun id -> 
                                let context = getContext()
                                let user = context.Public.User |> 
                                           Seq.tryFind (fun a -> a.Id = id)
                                match user with
                                    | Some u -> Some ({ Name = u.Name
                                                        UserName = u.UserName
                                                        Email = u.Email
                                                        Id = u.Id
                                                        AccessType = deserializeAccessType u.AccessType  } 
                                                        : Queries.User.Get.User)
                                    | None -> None ) id
let getUserByUserName userName =
    handleDatabaseException ( fun id -> 
                                let context = getContext()
                                let user = context.Public.User |> 
                                           Seq.tryFind (fun a -> a.UserName = userName)
                                match user with
                                    | Some u -> Some { UserName = u.UserName
                                                       Name = u.Name
                                                       Email = u.Email
                                                       Id = u.Id
                                                       Password = u.Password
                                                       AccessType = deserializeAccessType u.AccessType  }
                                    | None -> None ) userName
    
let isUserAdministratorAsync (userId : Guid) =    
    handleDatabaseExceptionAsync(
        fun userId -> async {        
            let context = getContext()
        
            let! user = 
                query {
                    for user in context.Public.User do
                    where (user.Id = userId)
                    select(user)
                } |> Seq.executeQueryAsync

            match user |> Seq.tryHead with 
                | Some u -> 
                    let accessType = deserializeAccessType u.AccessType 
                    return accessType = AccessType.Administrator
                | None -> return false         
    }) userId

let isUserAdministrator id =
    handleDatabaseException (
        fun id ->
            let context = getContext()
            let user = context.Public.User |> Seq.tryFind (fun u -> u.Id = id)
            match user with
                | Some u -> AccessType.Administrator = deserializeAccessType u.AccessType 
                | None -> false ) id
let getUserAccessType id =
    let context = getContext()
    let user = context.Public.User |> Seq.find (fun u -> u.Id = id)
    deserializeAccessType user.AccessType 
let isUserEmailUnused =
    handleDatabaseException 
        (fun email -> let context = getContext()
                      not ( context.Public.User |> 
                                        Seq.exists(fun u -> u.Email = email) ) ) 

let isUserNameUnused username =
    handleDatabaseException 
        (fun username -> let context = getContext()
                         not ( context.Public.User |> 
                                        Seq.exists(fun u -> u.UserName = username) ) ) username
        
let isUserEmailAvailable parameters =
    handleDatabaseException 
        (fun parameters -> let context = getContext()
                           not ( context.Public.User |> 
                                        Seq.exists(fun u -> u.Email = fst parameters && 
                                                            u.Id <> snd parameters) ) ) parameters

let isUserNameAvailable paramaters =
    handleDatabaseException 
        (fun paramaters -> let context = getContext()
                           not ( context.Public.User |> 
                                    Seq.exists(fun u -> u.UserName = fst paramaters && 
                                                        u.Id <> snd paramaters ) ) ) paramaters

let insertUser =
    handleDatabaseException
        ( fun (cmd : User.Create.Command) -> 
                     let context = getContext()
                     let newUser = context.Public.User.Create()    
                     newUser.AccessType <- serializeAccessType cmd.AccessType
                     newUser.Email <- cmd.Email
                     newUser.Id <- Guid.NewGuid()
                     newUser.Name <- cmd.Name
                     newUser.Password <- cmd.Password
                     newUser.UserName <- cmd.UserName
                     context.SubmitUpdates() ) 
    
let updateUser =
    handleDatabaseException
        ( fun (command' : User.Update.Command) -> 
            let context = getContext()
            let user = context.Public.User |> 
                       Seq.tryFind (fun g -> g.Id = command'.Id)   
            match user with
                | Some u -> u.AccessType <- (serializeAccessType command'.AccessType)
                            u.Email <- command'.Email
                            u.Name <- command'.Name
                            u.UserName <- command'.UserName
                            context.SubmitUpdates()
                | None -> 
                    raise (new Exception("Sentences.Validation.IdMustReferToAnExistingUser")) )     

let updateUserPassword (command: User.UpdatePassword.Command) =
    handleDatabaseException 
        ( fun (command : User.UpdatePassword.Command) -> 
                         let context = getContext()
                         let user = context.Public.User |> 
                                    Seq.tryFind (fun g -> g.Id = command.Id)   
                         match user with
                             | Some u -> u.Password <- command.Password
                                         context.SubmitUpdates()
                             | None -> raise (new Exception("Sentences.Validation.IdMustReferToAnExistingUser")) ) command
        
let deleteUser (command : User.Delete.Command) =
    handleDatabaseException 
        ( fun id -> let context = getContext()
                    let user = context.Public.User |> 
                               Seq.tryFind (fun g -> g.Id = command.Id)
                    match user with
                      | Some u -> u.Delete()
                                  context.SubmitUpdates()
                      | None -> raise (new Exception("Sentences.Validation.IdMustReferToAnExistingUser"))
                    context.SubmitUpdates() ) command

let grantPermission =
    handleDatabaseException
        ( fun (cmd : User.GrantPermission.Command) -> 
                     let context = getContext()
                     let newPermission = context.Public.Permission.Create()
                     newPermission.UserId <- cmd.UserId
                     newPermission.PackageId <- cmd.PackageId
                     context.SubmitUpdates() )

let revokePermission =
    handleDatabaseException
        ( fun (cmd : User.RevokePermission.Command) -> 
                     let context = getContext()

                     let permission = context.Public.Permission |> 
                                      Seq.tryFind(fun p -> p.UserId = cmd.UserId &&
                                                           p.PackageId = cmd.PackageId)
                     
                     match permission with | Some p -> p.Delete()
                                                       context.SubmitUpdates()
                                           | None -> () )

let permissionExists =
    handleDatabaseException
        ( fun args ->
            let userId = fst args
            let packageId = snd args
            let context = getContext()
            context.Public.Permission |> 
            Seq.exists (fun p -> p.UserId = userId && p.PackageId = packageId) )

let isUserObserver =
    handleDatabaseException
        ( fun userId  ->
            let context = getContext()
            context.Public.User |> 
            Seq.exists (fun u -> u.Id = userId && 
                                 u.AccessType = (serializeAccessType AccessType.User) ) )

let getPermissionsByPackage (query' : Queries.Permissions.ListByPackage.Query) =
    let context = getContext()
    context.Public.Permission |>
    Seq.filter (fun p -> p.PackageId = query'.PackageId ) |>
    Seq.map (fun p -> { UserId = p.UserId;  
                        PackageId = p.PackageId } : Queries.Permissions.ListByPackage.Permission) |>
    Seq.toList 

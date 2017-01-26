module PgSqlUserPersistence

open PgSqlPersistence
open Commands
open System
open Models
open Queries.ChallengeUserCredentials

let inline private serializeAccessType input =
    match input with
        | Administrator -> "admin"
        | User -> "user"

let inline private deserializeAccessType input =
    match input with 
        | "admin" -> Administrator
        | "user" -> User
        | _ -> raise (new Exception(Sentences.Error.InvalidAccessType))
        
let userExists id =
    handleDatabaseException ( fun id -> let context = getContext()
                                        context.Public.User |> Seq.exists (fun g -> g.Id = id) ) id
       
let getUserByUserName userName =
    handleDatabaseException ( fun id -> 
                                let context = getContext()
                                let user = context.Public.User |> 
                                           Seq.tryFind (fun u -> u.UserName = userName)
                                match user with
                                    | Some u -> Some { UserName = u.UserName
                                                       Name = u.Name
                                                       Email = u.Email
                                                       Password = u.Password
                                                       AccessType = deserializeAccessType u.AccessType  }
                                    | None -> None ) userName
    
let isUserAdministrator id =
    handleDatabaseException (
        fun id ->
            let context = getContext()
            let user = context.Public.User |> Seq.tryFind (fun u -> u.Id = id)
            match user with
                | Some u -> AccessType.Administrator = deserializeAccessType u.AccessType 
                | None -> false ) id

let isUserEmailUnused email =
    handleDatabaseException 
        (fun email -> let context = getContext()
                      not ( context.Public.User |> 
                                        Seq.exists(fun u -> u.Email = email) ) ) email

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

let insertUser (command: CreateUser.Command) =
    handleDatabaseException
        ( fun id -> let context = getContext()
                    let newUser = context.Public.User.Create()    
                    newUser.AccessType <- serializeAccessType command.AccessType
                    newUser.Email <- command.Email
                    newUser.Id <- Guid.NewGuid()
                    newUser.Name <- command.Name
                    newUser.Password <- command.Password
                    newUser.UserName <- command.UserName
                    context.SubmitUpdates() ) command
    
let updateUser (command: UpdateUser.Command) =
    handleDatabaseException
        ( fun (command' : UpdateUser.Command) -> 
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
                    raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser)) ) command        

let updateUserPassword (command: UpdateUserPassword.Command) =
    handleDatabaseException 
        ( fun (command : UpdateUserPassword.Command) -> 
                         let context = getContext()
                         let user = context.Public.User |> 
                                    Seq.tryFind (fun g -> g.Id = command.Id)   
                         match user with
                             | Some u -> u.Password <- command.Password
                                         context.SubmitUpdates()
                             | None -> raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser)) ) command
        
let deleteUser (command : DeleteUser.Command) =
    handleDatabaseException 
        ( fun id -> let context = getContext()
                    let user = context.Public.User |> 
                               Seq.tryFind (fun g -> g.Id = command.Id)
                    match user with
                      | Some u -> u.Delete()
                                  context.SubmitUpdates()
                      | None -> raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser))
                    context.SubmitUpdates() ) command


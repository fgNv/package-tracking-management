module PgSqlUserPersistence

open PgSqlPersistence
open Commands
open System
open Models
open Queries.User.ChallengeUserCredentials

let private serializeAccessType input =
    match input with
        | Administrator -> "admin"
        | User -> "user"

let private deserializeAccessType input =
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
                                                       Id = u.Id
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
                    raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser)) )     

let updateUserPassword (command: User.UpdatePassword.Command) =
    handleDatabaseException 
        ( fun (command : User.UpdatePassword.Command) -> 
                         let context = getContext()
                         let user = context.Public.User |> 
                                    Seq.tryFind (fun g -> g.Id = command.Id)   
                         match user with
                             | Some u -> u.Password <- command.Password
                                         context.SubmitUpdates()
                             | None -> raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser)) ) command
        
let deleteUser (command : User.Delete.Command) =
    handleDatabaseException 
        ( fun id -> let context = getContext()
                    let user = context.Public.User |> 
                               Seq.tryFind (fun g -> g.Id = command.Id)
                    match user with
                      | Some u -> u.Delete()
                                  context.SubmitUpdates()
                      | None -> raise (new Exception(Sentences.Validation.IdMustReferToAnExistingUser))
                    context.SubmitUpdates() ) command


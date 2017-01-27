module Application

module User =
    let Create =
        Commands.User.Create.handle 
            PgSqlUserPersistence.isUserAdministrator
            PgSqlUserPersistence.isUserEmailUnused
            PgSqlUserPersistence.isUserNameUnused
            PgSqlUserPersistence.insertUser

    let Update =
        Commands.User.Update.handle 
            PgSqlUserPersistence.isUserAdministrator
            PgSqlUserPersistence.isUserEmailAvailable
            PgSqlUserPersistence.isUserNameAvailable
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.updateUser

    let UpdatePassword =
        Commands.User.UpdatePassword.handle 
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.updateUserPassword

    let Delete =
        Commands.User.Delete.handle
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.deleteUser
    
    open Queries.ChallengeUserCredentials

    let ChallengeCredentials username password =         
        Queries.ChallengeUserCredentials.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username; Password = password}
            
    let Exists username  =
        Queries.UserExists.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username}
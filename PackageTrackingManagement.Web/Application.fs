module Application

module User =
    let Create =
        Commands.CreateUser.handle 
            PgSqlUserPersistence.isUserAdministrator
            PgSqlUserPersistence.isUserEmailUnused
            PgSqlUserPersistence.isUserNameUnused
            PgSqlUserPersistence.insertUser

    let Update =
        Commands.UpdateUser.handle 
            PgSqlUserPersistence.isUserAdministrator
            PgSqlUserPersistence.isUserEmailAvailable
            PgSqlUserPersistence.isUserNameAvailable
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.updateUser

    let UpdatePassword =
        Commands.UpdateUserPassword.handle 
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.updateUserPassword

    let Delete =
        Commands.DeleteUser.handle
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.deleteUser
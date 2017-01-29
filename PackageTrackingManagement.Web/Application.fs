﻿module Application

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
    
    open Queries.User

    let ChallengeCredentials username password =         
        ChallengeUserCredentials.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username; Password = password}
            
    let Exists username =
        UserExists.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username}

module Package =    
    let Create =
        Commands.Package.Create.handle
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.insertPackage

    let Update =
        Commands.Package.Update.handle
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.packageExists
            PgSqlPackagePersistence.updatePackage

    let Delete =
        Commands.Package.Delete.handle
            PgSqlPackagePersistence.packageExists
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.deletePackage
    
    let AddManualPoint =
        Commands.Package.AddManualPoint.handle
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.packageExists
            PgSqlPackagePersistence.insertManualPoint
                
    let RemoveManualPoint =
        Commands.Package.RemoveManualPoint.handle
            PgSqlPackagePersistence.manualPointExists
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.deleteManualPoint
    
    let GetList =
        Queries.Package.List.handle
            PgSqlPackagePersistence.getPackageList

    let GetDetails =
        Queries.Package.Details.handle
            PgSqlPackagePersistence.getPackageDetails
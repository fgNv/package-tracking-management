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
            PgSqlUserPersistence.isUserAdministrator
            PgSqlUserPersistence.deleteUser
    
    open Queries.User

    let GetById =
        Queries.User.Get.handle
            PgSqlUserPersistence.getUserById

    let GetList =
        Queries.User.List.handle
            PgSqlUserPersistence.getUserList

    let ChallengeCredentials username password =         
        ChallengeUserCredentials.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username; Password = password}
            
    let Exists username =
        UserExists.handle
            PgSqlUserPersistence.getUserByUserName
            {UserName = username}

    let GrantPermission =
        Commands.User.GrantPermission.handle
            PgSqlUserPersistence.userExists
            PgSqlUserPersistence.isUserObserver
            PgSqlPackagePersistence.packageExists
            PgSqlUserPersistence.grantPermission

    let RevokePermission =
        Commands.User.RevokePermission.handle
            PgSqlUserPersistence.permissionExists
            PgSqlUserPersistence.revokePermission
    
    let GetPermissionsByPackage =
        Queries.Permissions.ListByPackage.handle
            PgSqlUserPersistence.getPermissionsByPackage

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
        let externalValidations = {
            new Commands.Package.AddManualPoint.IExternalValidations 
                with member this.IsCreatorAdministrator id = PgSqlUserPersistence.isUserAdministratorAsync id
                     member this.PackageExists id = PgSqlPackagePersistence.packageExistsAsync id
        }

        Commands.Package.AddManualPoint.handle
            externalValidations
            PgSqlPackagePersistence.insertManualPoint
                
    let RemoveManualPoint =
        Commands.Package.RemoveManualPoint.handle
            PgSqlPackagePersistence.manualPointExists
            PgSqlUserPersistence.isUserAdministrator
            PgSqlPackagePersistence.deleteManualPoint
    
    let GetList =
        Queries.Package.List.handle
            PgSqlUserPersistence.getUserAccessType
            PgSqlPackagePersistence.getPackageList
            PgSqlPackagePersistence.getUserPackageList

    let GetDetails =
        Queries.Package.Details.handle
            PgSqlPackagePersistence.getPackageDetails
module Queries.Permissions

open Models
open Railroad
open System

open Chiron
open Chiron.Operators

module ListByPackage =
    type Query = { PackageId : Guid }

    type Permission = { UserId : Guid 
                        PackageId : Guid}
        with static member ToJson(x : Permission) =
               Json.write "userId" x.UserId
            *> Json.write "packageId" x.PackageId

    let handle (getPermissionList : Query -> Permission list) query =
        getPermissionList query

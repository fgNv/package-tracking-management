module Commands.Package

open Models
open Commands.Global
open System

module Create =
    type Command = {Name : string
                    Description : string
                    Status : PackageStatus
                    CreatorId : Guid}
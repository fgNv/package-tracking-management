module Routes

open Railroad
open Application
open JsonParse

open System
open Suave
open Suave.Successful
open Suave.RequestErrors
open Suave.ServerErrors
open Suave.Filters
open Suave.Writers
open Suave.CORS
open Actions

open Suave.Operators

let apiRoutes =
    let protectResource = 
        ResourceProtection.protectResource [|Suave.Authentication.UserNameKey; Claims.UserIdKey|]
    
    let setCORSHeaders =
        setHeader  "Access-Control-Allow-Origin" "*"
        >=> setHeader "Access-Control-Allow-Headers" "content-type"

    let setJsonHeaders = 
        setHeader "Content-type" "application/json"

    let corsConfig = { defaultCORSConfig with allowedUris = InclusiveOption.All
                                              allowedMethods = InclusiveOption.Some [HttpMethod.DELETE; HttpMethod.GET; HttpMethod.POST; HttpMethod.PUT; HttpMethod.OPTIONS]  }
    
    let jsonEndpoints = 
        protectResource (
            choose [
                 pathScan "/package/%s/permissions" (fun id ->
                    let query= { PackageId = Guid.Parse id } : Queries.Permissions.ListByPackage.Query
                    let list = Application.User.GetPermissionsByPackage query
                    OK (QueryResult.serializeList list)
                 )
                 DELETE >=> pathScan "/permission/user/%s/package/%s" (fun (userId, packageId) ->
                    let parsedUserId = Guid.Parse(userId)
                    let parsedPackageId = Guid.Parse(packageId)
                    let command = { UserId = parsedUserId
                                    PackageId = parsedPackageId } : Commands.User.RevokePermission.Command                    
                    match User.RevokePermission command with
                        | Success _ -> NO_CONTENT
                        | Error(_) -> INTERNAL_ERROR("error")
                 ) 
                 path "/permission" >=>                     
                    POST >=> 
                        Actions.grantPermission
                 path "/package/point/manual" >=> 
                        choose [ POST >=> context(Actions.createManualPoint) 
                                 DELETE >=> context(Actions.removeManualPoint) ]
                 pathScan "/package/%s" 
                   (fun id -> 
                      let parsedId = System.Guid.Parse id
                      choose [ 
                        GET >=> Actions.getPackageById parsedId
                        DELETE >=> context (Actions.deletePackage parsedId) ])
                 path "/package" >=>
                     choose [ 
                        GET >=> context(Actions.getPackages)
                        POST >=> context (Actions.createPackage) 
                        PUT >=> context (Actions.updatePackage) ]
                 pathScan "/user/%s" 
                   (fun id -> 
                      let parsedId = System.Guid.Parse id
                      choose [ DELETE >=> context (Actions.deleteUser parsedId)
                               GET >=> Actions.getUserById parsedId ])
                 path "/user" >=>
                        choose [ POST >=> context (Actions.createUser)
                                 PUT >=> context (Actions.updateUser)
                                 GET >=> request(Actions.getUsers) ] 
        ] )
        
    choose [ OPTIONS >=> cors corsConfig
             GET >=> path "/" >=> Files.browseFileHome "index.html"
             path "/token" >=> context(fun ctx ->
                                    let emptyCtx = {ctx with userState = Map.empty}
                                    let middleware = 
                                            AuthorizationServer.authorizationServerMiddleware 
                                               User.ChallengeCredentials
                                               Claims.getCustomClaims
                                               Claims.getClientData
                                    (fun ignore -> middleware(emptyCtx)) )
             Files.browseHome
             jsonEndpoints >=> setCORSHeaders >=> setJsonHeaders   
             NOT_FOUND ("no resource matches this path")
          ]

module AuthorizationServer

open Owin
open Suave.Owin
open Microsoft.Owin.Builder
open Microsoft.Owin.Security.OAuth
open Microsoft.Owin
open System
open Railroad
open System.Security.Claims
open Owin.Security.AesDataProtectorProvider
open Microsoft.Owin.Security.DataProtection
open Owin.Security.AesDataProtectorProvider.CrypticProviders

let private hostAppName = "bearerTokenAuthentication"

type private SimpleAuthenticationProvider<'a>(validateUserCredentials, 
                                              getCustomClaims : 'a -> (string * string) list) =
    inherit OAuthAuthorizationServerProvider()
    override this.ValidateClientAuthentication (context : OAuthValidateClientAuthenticationContext) =
        let f: Async<unit> = async { context.Validated() |> ignore }
        upcast Async.StartAsTask f 

    override this.GrantResourceOwnerCredentials(context: OAuthGrantResourceOwnerCredentialsContext) =        
        let f: Async<unit> = async {  
            let result = validateUserCredentials context.UserName context.Password
            match result with 
                | Success user -> 
                    let identity = new ClaimsIdentity(context.Options.AuthenticationType)
                    identity.AddClaim(new Claim("sub", context.UserName))
                    identity.AddClaim(new Claim("role", "user"))

                    getCustomClaims user |> List.iter(fun tuple -> Claims.addClaim tuple identity )
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", [| "*" |]); 

                    context.Validated(identity) |> ignore
                | Error (title, errors) -> 
                    context.SetError("authenticationFailure", "invalidCredentials")
        }
        upcast Async.StartAsTask f 

let authorizationServerMiddleware validateUserCredentials getCustomClaims =
    let serverOptions = new OAuthAuthorizationServerOptions(
                            AllowInsecureHttp = true,
                            TokenEndpointPath= new PathString("/token"),
                            AccessTokenExpireTimeSpan = TimeSpan.FromDays(1.0),
                            Provider = new SimpleAuthenticationProvider<'a>(validateUserCredentials, getCustomClaims) )
    
    let builder = new AppBuilder() :> IAppBuilder

    builder.SetDataProtectionProvider(new AesDataProtectorProvider(new Sha512ManagedFactory(), new Sha256ManagedFactory(), new AesManagedFactory()))
    builder.UseOAuthAuthorizationServer(serverOptions) |> ignore
    builder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll) |> ignore
    builder.UseAesDataProtectorProvider() |> ignore
    builder.Properties.["host.AppName"] <- hostAppName
    let owinApp = builder.Build()
    OwinApp.ofAppFunc "/" owinApp
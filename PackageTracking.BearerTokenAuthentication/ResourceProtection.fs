module ResourceProtection

open Microsoft.Owin.Security
open Microsoft.Owin.Security.DataHandler
open Microsoft.Owin.Security.DataProtection
open Suave
open Owin
open Microsoft.Owin.Builder
open Microsoft.Owin.Security.OAuth
open Microsoft.Owin.Security.Infrastructure
open Railroad
open Owin.Security.AesDataProtectorProvider
open Owin.Security.AesDataProtectorProvider.CrypticProviders

let private hostAppName = "bearerTokenAuthentication"

let private buildDefaultBearerOptions ()= 
    let app = AppBuilder() :> IAppBuilder        
    app.Properties.["host.AppName"] <- hostAppName

    app.SetDataProtectionProvider(AesDataProtectorProvider(
                                        Sha512ManagedFactory(), 
                                        Sha256ManagedFactory(), 
                                        AesManagedFactory()))
    app.UseAesDataProtectorProvider() |> ignore      
    
    let typeDef = typedefof<OAuthAuthorizationServerMiddleware>
    let defaultDataProtector = app.CreateDataProtector(typeDef.Namespace, "Access_Token", "v1")
    
    let defaultAccessTokenFormat = TicketDataFormat(defaultDataProtector)
    let defaultOptions = OAuthBearerAuthenticationOptions(
                                        AccessTokenProvider = AuthenticationTokenProvider(),
                                        AccessTokenFormat = defaultAccessTokenFormat,
                                        Provider = OAuthBearerAuthenticationProvider() )  

    app.UseOAuthBearerAuthentication(defaultOptions) |> ignore
    defaultOptions

let private defaultOptions = buildDefaultBearerOptions()

let private validateToken requestToken =        
    let unprotectedTicket = defaultOptions.AccessTokenFormat.Unprotect(requestToken)
    Option.ofObj unprotectedTicket |> errorOnNone
    
let private extractToken (content : string) =
    let startLength = "Bearer ".Length
    let requestToken = content.Substring(startLength).Trim()
    let requestTokenContext = OAuthRequestTokenContext(null, requestToken);
    errorOnEmptyString requestTokenContext.Token

let private verifyTokenExpiration (ticket : AuthenticationTicket) =
    let currentUtc = defaultOptions.SystemClock.UtcNow
    match Option.ofNullable ticket.Properties.ExpiresUtc with
        | None -> 
            Error (TitleMessages("invalidToken", [|"tokenWithoutExpirationDate"|]))
        | Some expirationUtc when expirationUtc < currentUtc ->
            Error (TitleMessages("invalidToken", [|"tokenExpired"|]))
        | Some expirationUtc -> Success ticket
            
let private executeVerifications authorizationHeaderContent =                         
    authorizationHeaderContent |> 
    extractToken >>=
    validateToken >>=
    verifyTokenExpiration
    
let private getAuthorizationHeaderFromContext ctx =        
    match ctx.request.header "Authorization" with
          | Choice1Of2 header -> Success header
          | Choice2Of2 _ -> Error (TitleMessages("authenticationFailure", 
                                                 [| "noAuthenticationHeaderFound" |]))

let private getClaim (ticket : AuthenticationTicket) (key : string) =
    let claim = ticket.Identity.Claims |> Seq.tryFind(fun c -> c.Type = key)
    match claim with 
        | Some c -> Success c.Value 
        | None -> Error (TitleMessages("sem claim", [|"CLAIMLESS"|]))
                
let private addClaims ticket (ctx : HttpContext) (claimsKeys : string seq) = 
    let claimsRetrieved = claimsKeys |> Seq.map (fun key -> key, (getClaim ticket key))

    let anyWithError = claimsRetrieved |> Seq.exists (fun (key, value) -> Railroad.isError value)

    match anyWithError with
        | false -> 
            let choosed = claimsRetrieved |> Seq.choose
                                               (fun (key, value) -> match value with 
                                                                     | Success v -> Some (key, box(v)) 
                                                                     | _ -> None)   
                                          |> Array.ofSeq      
            Success { ctx with userState = Map.ofArray choosed }
        | true -> Error (TitleMessages("rr", ["rr"]) )

let protectResource (claimsKeys : string seq) (protectedPart : WebPart) (ctx : HttpContext) =
    let result = ctx |> getAuthorizationHeaderFromContext >>= executeVerifications
    
    match result with
    | Success authTicket ->                         
        let contextWithClaims = addClaims authTicket ctx claimsKeys
        match contextWithClaims with
            | Success context -> protectedPart context
            | Error _ -> Suave.RequestErrors.challenge ctx
    | Error _ -> 
        Suave.RequestErrors.challenge ctx

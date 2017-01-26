module Models

open Chiron

type AccessType = | Administrator | User
    with
        static member FromJson (_:AccessType) = json {
                                let! accessType = Json.read "accessType"
                                match accessType with
                                    | "administrator" -> return AccessType.Administrator
                                    | "user" -> return AccessType.User
                                    | _ -> return! Json.error "not a access type"
                               }
        static member ToJson(x : AccessType) = json {
            match x with
                | Administrator -> do! Json.write "accessType" "administrator"
                | User -> do! Json.write "accessType" "user"
        }

module Password =
    open System.Security.Cryptography
    open System.Text

    let inline getPassword (a: ^a when ^a:(member GetPassword : unit -> string)) =
        (^a: (member GetPassword: unit -> string)(a))    

    let inline getEncryptedPassword input =
        let password = getPassword input
        let data = Encoding.ASCII.GetBytes(password);
        let shaM = new SHA256Managed()
        let encryptedBytes = shaM.ComputeHash(data)
        Encoding.ASCII.GetString encryptedBytes


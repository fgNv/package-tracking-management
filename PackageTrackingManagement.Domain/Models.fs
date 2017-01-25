module Models

type AccessType = | Administrator | User

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


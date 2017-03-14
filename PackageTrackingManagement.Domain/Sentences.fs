module Sentences

open FSharp.Data
open System
open System.IO

type Language = | PtBr
                | EnUs

type Sentence = | CountryCodeIsRequired
                | CouldNotAccessDatabase
                | CouldNotAuthenticate
                | DatabaseFailure
                | ErrorValidatingCredentials
                | IdMustReferToAnExistingUser
                | IdMustReferToExistingPackage
                | IdMustReferToExistingManualPoint
                | IdMustReferToExistingPoint
                | IdMustReferToAnExistingPermission
                | InvalidAccessType
                | InvalidData
                | InvalidInputContent
                | InvalidCredentials
                | LatitudeMustBeBetweenMinusNinetyAndNinety
                | LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty
                | OnlyAdministratorsMayPerformThisAction
                | PasswordIsRequired
                | ThisUserNameIsNotAvailable
                | ThisEmailIsNotAvailable
                | UserNameIsRequired
                | UserMayNotDeleteHimself
                | UserMustBeObserver

let translate language sentence =
    match sentence with 
        | CountryCodeIsRequired -> 
            match language with 
                | PtBr -> "Código do país é obrigatório"
                | EnUs -> "Country code is required"      
        | CouldNotAccessDatabase ->
            match language with
                | PtBr -> "Não foi possível acessar o banco de dados"
                | EnUs -> "Could not access database"
        | CouldNotAuthenticate -> 
            match language with 
                | PtBr -> "Não foi possível autenticar"
                | EnUs -> "Could not authenticate"
        | DatabaseFailure -> 
            match language with 
                | PtBr -> "Falha no banco de dados"
                | EnUs -> "Database failure"
        | ErrorValidatingCredentials -> 
            match language with 
                | PtBr -> "Erro ao validar credenciais"
                | EnUs -> "Error validating credentials"
        | IdMustReferToExistingManualPoint -> 
            match language with 
                | PtBr -> "Id deve ser referente a um ponto manual existente"
                | EnUs -> "Id must refer to existing manual point"
        | IdMustReferToExistingPoint -> 
            match language with 
                | PtBr -> "Id deve ser referente a um ponto existente"
                | EnUs -> "Id must refer to existing point"        
        | IdMustReferToAnExistingPermission -> 
            match language with 
                | PtBr -> "Id deve ser referente a uma permissão existente"
                | EnUs -> "Id must refer to an existing permission"
        | IdMustReferToAnExistingUser -> 
            match language with 
                | PtBr -> "Id deve ser referente a um usuário existente"
                | EnUs -> "Id must refer to an existing user"
        | IdMustReferToExistingPackage -> 
            match language with 
                | PtBr -> "Id deve ser referente a um pacote existente"
                | EnUs -> "Id must refer to existing package"
        | InvalidData -> 
            match language with 
                | PtBr -> "Dados inválidos"
                | EnUs -> "Invalid data"  
        | InvalidInputContent ->
            match language with 
                | PtBr -> "Valor de entrada inválido"
                | EnUs -> "Invalid input content"  
        | InvalidAccessType ->
            match language with 
                | PtBr -> "Tipo de acesso inválido"
                | EnUs -> "Invalid access type"  
        | InvalidCredentials -> 
            match language with 
                | PtBr -> "Credenciais inválidas"
                | EnUs -> "Invalid credentials"
        | LatitudeMustBeBetweenMinusNinetyAndNinety -> 
            match language with 
                | PtBr -> "Latitude deve estar entre -90 e 90"
                | EnUs -> "Latitude must be between minus ninety and ninety"
        | LongitudeMustBeBetweenMinusOneHundredEightyAndOneHundredEighty -> 
            match language with 
                | PtBr -> "Longitude deve estar entre -180 e 180"
                | EnUs -> "Longitude must be between minus one hundred eighty and one hundred eighty"
        | OnlyAdministratorsMayPerformThisAction -> 
            match language with 
                | PtBr -> "Apenas administradores podem executar essa ação"
                | EnUs -> "Only administrators may perform this action"
        | ThisEmailIsNotAvailable -> 
            match language with 
                | PtBr -> "Esse e-mail não está disponível"
                | EnUs -> "This email is not available"
        | ThisUserNameIsNotAvailable ->
            match language with 
                | PtBr -> "Esse nome de usuário não está disponível"
                | EnUs -> "This username is notbavailable"        
        | UserMayNotDeleteHimself -> 
            match language with 
                | PtBr -> "Usuário não pode se deletar"
                | EnUs -> "User may not delete himself"
        | UserMustBeObserver -> 
            match language with 
                | PtBr -> "Usuário deve ser observador"
                | EnUs -> "User must be observer"
        | UserNameIsRequired -> 
            match language with 
                | PtBr -> "Deve-se informar o nome"
                | EnUs -> "Username is required"
        | PasswordIsRequired -> 
            match language with 
                | PtBr -> "Deve-se informar a senha"
                | EnUs -> "Password is required"
  
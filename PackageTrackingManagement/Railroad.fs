module Railroad

open System

type ErrorContent =
    | Title of string
    | TitleMessages of string * seq<string>

type Result<'TEntity> =  
    | Success of 'TEntity
    | Error of string * seq<string>
    
let isError result =
    match result with | Success _ -> false | Error _ -> true

let bind switchFunction input =
    match input with
        | Success result -> switchFunction result
        | Error (description, errors) -> Error (description, errors)

let (>>=) input switchFunction =
    bind switchFunction input

let errorOnEmptySeq seq =
    match seq |> Seq.isEmpty with
        | true -> Error ("failure", [|"emptySequence"|]) | false -> Success seq

let inline errorOnNone input =
    match input with 
        | Some v -> Success v
        | None -> Error("failure", [|"None"|])

let errorOnEmptyString string =
    match string |> String.IsNullOrWhiteSpace with
        | true -> Error ("failure", [|"emptyString"|]) | false -> Success string
        

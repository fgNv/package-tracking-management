module Railroad

open System

type ErrorContent =
    | Title of string
    | TitleMessages of string * string seq
    | TitleSentence of Sentences.Sentence
    | TitleSentenceMessages of Sentences.Sentence * Sentences.Sentence seq
    | TitleSentenceMessagesStr of Sentences.Sentence * string seq

type TranslatedErrorContent = {Title : string; Messages : string seq}

let GetErrorContentTup language content =
    let translate = Sentences.translate language
    match content with
        | Title title -> (title , Seq.empty<string>)
        | TitleMessages (title, messages) -> (title, messages)
        | TitleSentence title -> (translate title, Seq.empty)
        | TitleSentenceMessages (title, messages) -> (translate title,
                                                       messages |> Seq.map translate)
        | TitleSentenceMessagesStr (title, messages) -> (translate title, messages)

let GetErrorContent language content =
    let (title, messages) = GetErrorContentTup language content
    {Title = title; Messages = messages}

type Result<'TEntity> =  
    | Success of 'TEntity
    | Error of ErrorContent
    
let isError result =
    match result with | Success _ -> false | Error _ -> true

let bind switchFunction input =
    match input with
        | Success result -> switchFunction result
        | Error content -> Error content

let (>>=) input switchFunction =
    bind switchFunction input

let (>=>) switch1 switch2 x = 
    match switch1 x with
    | Success s -> switch2 s
    | Error content -> Error content

let errorOnEmptySeq seq =
    match seq |> Seq.isEmpty with
        | true -> Error (TitleMessages("failure", [|"emptySequence"|])) | false -> Success seq

let inline errorOnNone input =
    match input with 
        | Some v -> Success v
        | None -> Error (TitleMessages("failure", [|"None"|]))

let errorOnEmptyString string =
    match string |> String.IsNullOrWhiteSpace with
        | true -> Error (TitleMessages("failure", [|"emptyString"|])) | false -> Success string
        

module Sentences

open FSharp.Data

type PtBrValidationSentences = JsonProvider<"Resources/validation.pt-BR.json", ResolutionFolder = __SOURCE_DIRECTORY__>
type PtBrErrorSentences = JsonProvider<"Resources/error.pt-BR.json", ResolutionFolder = __SOURCE_DIRECTORY__ >

let Validation = 
    PtBrValidationSentences.Load("Resources/validation.pt-BR.json")

let Error = 
    PtBrErrorSentences.Load("Resources/error.pt-BR.json")
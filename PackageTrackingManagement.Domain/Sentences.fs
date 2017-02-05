module Sentences

open FSharp.Data

type PtBrValidationSentences = JsonProvider<"validation.pt-BR.json">
type PtBrErrorSentences = JsonProvider<"error.pt-BR.json">

let Validation = 
    PtBrValidationSentences.Load("validation.pt-BR.json")

let Error = 
    PtBrErrorSentences.Load("error.pt-BR.json")
module Sentences

open FSharp.Data

type PtBrValidationSentences = JsonProvider<"../PackageTrackingManagement.Domain/validation.pt-BR.json">
type PtBrErrorSentences = JsonProvider<"../PackageTrackingManagement.Domain/error.pt-BR.json">

let Validation = 
    PtBrValidationSentences.Load(__SOURCE_DIRECTORY__ + "/validation.pt-BR.json")

let Error = 
    PtBrErrorSentences.Load(__SOURCE_DIRECTORY__ + "/error.pt-BR.json")
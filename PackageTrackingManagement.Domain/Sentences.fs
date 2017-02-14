module Sentences

open FSharp.Data

type PtBrValidationSentences = JsonProvider<"../PackageTrackingManagement.Domain/Resources/validation.pt-BR.json">
type PtBrErrorSentences = JsonProvider<"../PackageTrackingManagement.Domain/Resources/error.pt-BR.json">

let Validation =
   PtBrValidationSentences.Load("Resources/validation.pt-BR.json")

let Error = 
   PtBrErrorSentences.Load("Resources/error.pt-BR.json")
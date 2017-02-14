module Sentences

open FSharp.Data
open System
open System.IO

type PtBrValidationSentences = JsonProvider<"Resources/validation.pt-BR.json">
type PtBrErrorSentences = JsonProvider<"Resources/error.pt-BR.json">

let private getResolutionFolder() =
    let resolutionPath = Environment.GetEnvironmentVariable("json_resolution_path")
    System.Console.WriteLine("json resolution path -> " + resolutionPath)
    resolutionPath

let Validation =
    let resPath = getResolutionFolder()
    let combined = Path.Combine(resPath, "validation.pt-BR.json")
    PtBrValidationSentences.Load(combined)

let Error = 
    let resPath = getResolutionFolder()
    let combined = Path.Combine(resPath, "error.pt-BR.json")
    PtBrErrorSentences.Load(combined)
module Commands.Global

open Sentences

module Validation =
    let inline validate getErrors input =
        let errors = getErrors input

        match errors |> Seq.isEmpty with
                | true -> Railroad.Result.Success input
                | false -> Railroad.Result.Error (translate Language.PtBr Sentence.InvalidData, errors)
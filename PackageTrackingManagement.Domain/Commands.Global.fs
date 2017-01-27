module Commands.Global

open Railroad

module Validation =
    let inline validate getErrors input =
        let errors = getErrors input

        match errors |> Seq.isEmpty with
                | true -> Result.Success input
                | false -> Result.Error (Sentences.Validation.InvalidData, errors)
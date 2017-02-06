module Commands.Global

module Validation =
    let inline validate getErrors input =
        let errors = getErrors input

        match errors |> Seq.isEmpty with
                | true -> Railroad.Result.Success input
                | false -> Railroad.Result.Error ("Sentences.Validation.InvalidData", errors)
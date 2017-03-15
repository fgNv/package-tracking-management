module EnvironmentVariables

open System.IO
open DotEnvFile

let loadEnvData () =
    let pathToFile = Path.Combine(__SOURCE_DIRECTORY__ , ".env")    
    match File.Exists(pathToFile) with
        | true -> let variables = DotEnvFile.LoadFile(pathToFile)
                  DotEnvFile.InjectIntoEnvironment(variables)
        | false -> ()
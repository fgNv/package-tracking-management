module Error 

let rec private getExceptionMessages' (ex : System.Exception, result : List<string>) =
    match ex.InnerException with 
        | null -> ex.Message :: result
        | inner -> getExceptionMessages'(inner, ex.Message :: result)

let getExceptionMessages (ex : System.Exception) =
    getExceptionMessages'(ex, [])
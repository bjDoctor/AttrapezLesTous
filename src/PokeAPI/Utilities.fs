//////
/// Helper functions used in the APIs module
//////

module APIs.Utilities

open System


/// Custom path concatenation operator
let (+/) (lhs: string) (rhs: string) = 
    let slashes = ['/'; '\\']

    let lastLhsChar = lhs |> Seq.last
    let firstRhsChar = rhs |> Seq.head

    let trimmedLhs = if slashes |> List.contains lastLhsChar 
                        then lhs.[..(lhs.Length - 2)]
                        else lhs

    let trimmedRhs = if slashes |> List.contains firstRhsChar 
                        then rhs.[1..]
                        else rhs

    trimmedLhs + "/" + trimmedRhs 


/// Replcace escape characters by a space
let filterOutEscapeCharacters input =
    let pattern = @"[\n\f]"
    
    try
        System.Text.RegularExpressions.Regex.Replace(input, pattern, " ")
    with 
        | _ ->  if isNull input then String.Empty else input //catch any exception


/// Callable from C#: converts an F# async into a C# task
let startAsyncFunctionAsTask f x y =
    async {
        return! f x y
    }
    |> Async.StartAsTask
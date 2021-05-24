//////
/// Helper functions used in this modules
/// These functions are testable at a unit level and have no side effects
//////

module Core.Utilities

open System

/// Custom path concatenation operator
let (+/) (lhs: string) rhs = 
    let rhsString = rhs |> string
    let slashes = ['/'; '\\']

    let lastLhsChar = lhs |> Seq.last
    let firstRhsChar = rhsString |> Seq.head

    let trimmedLhs = if slashes |> List.contains lastLhsChar 
                        then lhs.[..(lhs.Length - 2)]
                        else lhs

    let trimmedRhs = if slashes |> List.contains firstRhsChar 
                        then rhsString.[1..]
                        else rhsString

    trimmedLhs + "/" + trimmedRhs 


/// Replcaces escape characters by a space
let filterOutEscapeCharacters input =
    let pattern = @"[\n\f]"
    
    try
        System.Text.RegularExpressions.Regex.Replace(input, pattern, " ")
    with 
        | _ ->  if isNull input then String.Empty else input //catch any exception
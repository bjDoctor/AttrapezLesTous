//////
/// Helper functions used in the APIs module
//////

module APIs.Utilities

open System
open System.Collections.Generic
open Domain.Types

/////

//TODO: move to separate module 

// Initialise cache
let pokemonDetailsCache = new Dictionary<string, PokemonDetails>()

let getFromCacheOrFetch (cache: Dictionary<'a, 'b>) fetch key = 
    let fetchAndUpdateCache() = 
        async {
            let! value = fetch key
            cache.TryAdd(key, value) |> ignore
            return value
        }

    async {
        match cache.TryGetValue(key) with
            | true, cachedDetails -> return cachedDetails 
            | _ -> return! fetchAndUpdateCache()
    }

/////


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
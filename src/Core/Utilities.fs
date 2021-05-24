//////
/// Helper functions used in this modules
/// These functions are testable at a unit level and have no side effects
//////

module Core.Utilities

open System
open Core.Domain
open Core.JsonProviders

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


/// Builds a Pokemon object from a raw response of the pokeapi request
/// Logs an error if an exception occurs and re-throws
let makePokemon log (rawPokemonSpecies: PokemonSpeciesProvider.Root)  =
    try
        let englishDescription = 
            rawPokemonSpecies.FlavorTextEntries 
            |> Array.find(fun x-> x.Language.Name="en")

        {
            Name = rawPokemonSpecies.Name; 
            Description = filterOutEscapeCharacters englishDescription.FlavorText; 
            Habitat = rawPokemonSpecies.Habitat.Name; 
            IsLegendary = rawPokemonSpecies.IsLegendary
        }
    with
        | ex -> 
            log $"Failed make a Pokemon object: {ex.Message}"
            reraise()
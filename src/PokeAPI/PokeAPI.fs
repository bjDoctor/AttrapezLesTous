//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module APIs.PokeAPI

open System
open APIs.Utilities
open APIs.JsonProviders
open Domain.Types

let private baseUrl = "https://pokeapi.co/api/v2/"

let private fetchPokemonIdAsync name = 
    async {
        let url = baseUrl +/ "pokemon" +/ name
        let! rawPokemon = PokemonProvider.AsyncLoad(url)
        
        return rawPokemon.Id |> string
    }

let private fecthPokemonAsync name =
    async{
        let! pokemonId = fetchPokemonIdAsync name

        let url = baseUrl +/ "pokemon-species" +/ pokemonId
        let! pokemonSpecies = JsonProviders.PokemonSpeciesProvider.AsyncLoad(url)

        let description = pokemonSpecies.FlavorTextEntries |> Array.find(fun x-> x.Language.Name="en" && x.Version.Name="blue" )

        return {
            Name = name; 
            Description = filterOutEscapeCharacters description.FlavorText; 
            Habitat = pokemonSpecies.Habitat.Name; 
            IsLegendary = pokemonSpecies.IsLegendary
            }
        }

/// Callable from C#
let private startAsyncFunctionAsTask f x =
    async {
        return! f x
    }
    |> Async.StartAsTask

let GetPokemonAsync name = 
    startAsyncFunctionAsTask fecthPokemonAsync name
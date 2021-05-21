//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module APIs.PokeAPI

open System
open APIs.Utilities
open APIs.YodaAPI
open APIs.JsonProviders
open Domain.Types

let private pokeBaseUrl = "https://pokeapi.co/api/v2/"

let private fetchPokemonIdAsync name = 
    async {
        let url = pokeBaseUrl +/ "pokemon" +/ name
        let! rawPokemon = PokemonProvider.AsyncLoad(url)
        
        return rawPokemon.Id |> string
    }

let private fecthPokemonAsync name =
    async {
        let! pokemonId = fetchPokemonIdAsync name

        let url = pokeBaseUrl +/ "pokemon-species" +/ pokemonId
        let! pokemonSpecies = PokemonSpeciesProvider.AsyncLoad(url)

        let description = pokemonSpecies.FlavorTextEntries |> Array.find(fun x-> x.Language.Name="en" && x.Version.Name="blue" )

        return {
                Name = name; 
                Description = filterOutEscapeCharacters description.FlavorText; 
                Habitat = pokemonSpecies.Habitat.Name; 
                IsLegendary = pokemonSpecies.IsLegendary
            }
        }


let yodaTranslateDescriptionAsync (pokemon: Pokemon) = 
    async {
        let! yodaDescription = yodaTranslateAsync pokemon.Description
        return {pokemon with Description = yodaDescription}
    }

let private fetchTranslatedPokemonAsync name = 
    async {
        let! pokemon = fecthPokemonAsync name

        try
            return!
                match (pokemon.Habitat, pokemon.IsLegendary) with
                    | ("cave", _) | (_, true) -> yodaTranslateDescriptionAsync pokemon
                    | _ -> yodaTranslateDescriptionAsync pokemon
        with 
            | :? Exception as ex -> return pokemon //catch any exception: return default pokemon
    }


let GetPokemonAsync name = 
    startAsyncFunctionAsTask fecthPokemonAsync name

let GetTranslatedPokemonAsync name = 
    startAsyncFunctionAsTask fetchTranslatedPokemonAsync name
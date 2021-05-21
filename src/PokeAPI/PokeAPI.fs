//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module APIs.PokeAPI

open System
open Microsoft.Extensions.Logging
open APIs.Utilities
open APIs.YodaAPI
open APIs.JsonProviders
open Domain.Types

let private pokeBaseUrl = "https://pokeapi.co/api/v2/"

let private fetchPokemonId name = 
    async {
        let url = pokeBaseUrl +/ "pokemon" +/ name
        let! rawPokemon = PokemonProvider.AsyncLoad(url)
        
        return rawPokemon.Id |> string
    }

let private fecthPokemon logger name =
    async {
        let! pokemonId = fetchPokemonId name

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


let private fetchTranslatedDescription (pokemon: Pokemon) translation = 
    async {
        let! yodaDescription = fetchTranslation pokemon.Description translation
        return {pokemon with Description = yodaDescription}
    }

let private fetchTranslatedPokemon (logger: ILogger) name = 
    async {
        let! pokemon = fecthPokemon logger name

        try
            return!
                match (pokemon.Habitat, pokemon.IsLegendary) with
                    | ("cave", _) | (_, true) -> fetchTranslatedDescription pokemon Yoda
                    | _ -> fetchTranslatedDescription pokemon Shakespeare
        with 
            | ex -> logger.LogInformation($"Failed to translate description, returning non-translated description: {ex.Message}. ."); return pokemon //catch any exception
    }


let GetPokemonAsync logger name = 
    startAsyncFunctionAsTask fecthPokemon logger name

let GetTranslatedPokemonAsync logger name = 
    startAsyncFunctionAsTask fetchTranslatedPokemon logger name
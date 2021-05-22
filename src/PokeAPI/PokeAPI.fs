//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module Core.PokeAPI

open Microsoft.Extensions.Logging
open Core.Utilities
open Core.YodaAPI
open Core.JsonProviders
open Core.Domain
open System.Collections.Concurrent



let private _baseUrl = "https://pokeapi.co/api/v2/"
let private _pokemonSpeciesEndpoint = _baseUrl +/ "pokemon-species"

// Initialise cache
let private _pokemonCache = ConcurrentDictionary<string, Pokemon>()


let private makePokemon (logger: ILogger) (rawPokemonSpecies: PokemonSpeciesProvider.Root)  =
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
            logger.LogInformation($"Failed make a Pokemon object: {ex.Message}")
            reraise()

    

let private fecthPokemon logger name =
    let fetch name = async {
        let! rawPokemonSpecies = PokemonSpeciesProvider.AsyncLoad(_pokemonSpeciesEndpoint +/ name)

        return makePokemon logger rawPokemonSpecies
    }

    getFromCacheOrFetch _pokemonCache fetch name 


let private fetchTranslatedDescription (pokemon: Pokemon) translation = 
    async {
        let! yodaDescription = fetchTranslation pokemon.Description translation
        return {pokemon with Description = yodaDescription}
    }


/// Helper function allowing logging
let private fetchYodaTranslatedDescription  (logger: ILogger) pokemon = 
    logger.LogInformation($"Translating {pokemon.Name}'s description using master Yoda's dialect")
    fetchTranslatedDescription pokemon Yoda


/// Helper function allowing logging
let private fetchShakespeareTranslatedDescription  (logger: ILogger) pokemon = 
    logger.LogInformation($"Translating {pokemon.Name}'s description using Shakespearean English")
    fetchTranslatedDescription pokemon Shakespeare


let private fetchTranslatedPokemon (logger: ILogger) name = 
    async {
        let! pokemon = fecthPokemon logger name 

        try
            return!
                match (pokemon.Habitat, pokemon.IsLegendary) with
                    | ("cave", _) | (_, true) -> fetchYodaTranslatedDescription logger pokemon
                    | _ -> fetchShakespeareTranslatedDescription logger pokemon
        with 
            | ex -> logger.LogInformation($"Failed to translate description, returning non-translated description: {ex.Message}"); return pokemon //catch any exception
    }


let GetPokemonAsync logger name = 
    let f() = fecthPokemon logger name
    startAsyncFunctionAsTask f

let GetTranslatedPokemonAsync logger name = 
    let f() = fetchTranslatedPokemon logger name
    startAsyncFunctionAsTask f
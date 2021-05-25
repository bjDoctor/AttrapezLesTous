namespace Core

open Core.JsonProviders
open Core.Utilities
open Core.Domain
open System.Collections.Concurrent
open Microsoft.Extensions.Logging

//////
/// Module implementing retrieval of Pokemon detials through calls to the Poke Api and filtering of the returned data
/// Since it consists of retrieving data through the internet, this module is not tested at a unit level
/// Naming: We follow the convention fetch<Something> to convey the meaning that the undelying operation is asynchronous. Fetch clearly indicates the fact that we have to wait for "some" time and in the end we'll get something back.
/// (Although when I play fetch with my puppy, sometimes it feels like "fire and forget"!)
//////

module internal PokeApi = 
    // Constants 
    let private _baseUrl = "https://pokeapi.co/api/v2/"
    let private _pokemonSpeciesEndpoint = _baseUrl +/ "pokemon-species"

    // Initialise cache
    // This is a very basic cache, with no expiry time: once a pokemon has been inserted it can no longer be modified.
    // That's fine since we do not expect the data to change
    let private _pokemonCache = ConcurrentDictionary<string, Pokemon>()
    let private _translatedPokemonCache = ConcurrentDictionary<string, Pokemon>()


    /// Main function of this module: retrieves the pokemon details through a call to the poke api, and builds a pokemon object from the raw json response
    /// Note that this function also maintains a cache of Pokemons
    let fecthPokemon (logger: ILogger) name =
        let fetch() = async {
            let! rawPokemonSpecies = PokemonSpeciesProvider.AsyncLoad(_pokemonSpeciesEndpoint +/ name)

            return makePokemon logger.LogInformation rawPokemonSpecies
        }

        Cache.getFromCacheOrFetch _pokemonCache fetch name 


    /// Helper wrapper, deferring calls to the funtranslation api and updated the Pokemon's description
    let private fetchTranslatedDescription (pokemon: Pokemon) translation = 
        let fetch() = async {
            let! translatedDescription = TranslationApi.fetchTranslation pokemon.Description translation
            return {pokemon with Description = translatedDescription}
        }

        Cache.getFromCacheOrFetch _translatedPokemonCache fetch pokemon.Name 


    /// Helper function allowing logging
    let private fetchYodaTranslatedDescription  (logger: ILogger) pokemon = 
        logger.LogInformation($"Translating {pokemon.Name}'s description using master Yoda's dialect")
        fetchTranslatedDescription pokemon Yoda


    /// Helper function allowing logging
    let private fetchShakespeareTranslatedDescription  (logger: ILogger) pokemon = 
        logger.LogInformation($"Translating {pokemon.Name}'s description using Shakespearean English")
        fetchTranslatedDescription pokemon Shakespeare


    /// Retrieves a pokemon and translates its description
    /// Also maintains a cache of translated pokemons
    let fetchTranslatedPokemon (logger: ILogger) name = 
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

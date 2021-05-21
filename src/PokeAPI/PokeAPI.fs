//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module APIs.PokeAPI

open Microsoft.Extensions.Logging
open APIs.Utilities
open APIs.YodaAPI
open APIs.JsonProviders
open Domain.Types


let private pokeBaseUrl = "https://pokeapi.co/api/v2/"


let private fetchPokemonDetails name = 
    let fetch name = async {
        let url = pokeBaseUrl +/ "pokemon" +/ name
        let! rawPokemon = PokemonProvider.AsyncLoad(url)
        
        return {
            Id = rawPokemon.Id; 
            Name = rawPokemon.Name
        }
    }

    getFromCacheOrFetch pokemonDetailsCache fetch name 

let private makePokemon (logger: ILogger) (pokemonSpecies: PokemonSpeciesProvider.Root) name  =
    try
        let englishDescription = 
            pokemonSpecies.FlavorTextEntries 
            |> Array.find(fun x-> x.Language.Name="en")

        {
            Name = name; 
            Description = filterOutEscapeCharacters englishDescription.FlavorText; 
            Habitat = pokemonSpecies.Habitat.Name; 
            IsLegendary = pokemonSpecies.IsLegendary
        }
    with
        | ex -> 
            logger.LogInformation($"Failed make a Pokemon object: {ex.Message}")
            reraise()

    

let private fecthPokemon logger name =
    async {
        let! pokemonDetails = fetchPokemonDetails name

        let url = pokeBaseUrl +/ "pokemon-species" +/ pokemonDetails.Id
        let! pokemonSpecies = PokemonSpeciesProvider.AsyncLoad(url)

        return makePokemon logger pokemonSpecies pokemonDetails.Name 
    }


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
    startAsyncFunctionAsTask fecthPokemon logger name

let GetTranslatedPokemonAsync logger name = 
    startAsyncFunctionAsTask fetchTranslatedPokemon logger name
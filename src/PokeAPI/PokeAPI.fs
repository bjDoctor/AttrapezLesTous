//////
/// Module implementing retrieval of Pokemon detials through a series of REST API calls to the pokeapi and filtering of the returned data
//////

module APIs.PokeAPI

open System
open APIs.Utilities
open APIs.JsonProviders
open Domain.Types

let private baseUrl = "https://pokeapi.co/api/v2/"

let fetchPokemonId name = 
    async {
        let url = baseUrl +/ "pokemon" +/ name
        let! rawPokemon = PokemonProvider.AsyncLoad(url)
        
        return rawPokemon.Id
    }

let fecthPokemon name =
    async{
        let pokemonId = fetchPokemonId name

        let url = baseUrl +/ "pokemon-species" +/ pokemonId
        let! pokemonSpecies = JsonProviders.PokemonSpeciesProvider.AsyncLoad(url)
        let description = pokemonSpecies.FlavorTextEntries |> Array.find(fun x-> x.Language.Name="en" && x.Version.Name="blue" )

        return {
            Name = name; 
            Description = description.FlavorText; 
            Habitat = pokemonSpecies.Habitat.Name; 
            IsLegendary = pokemonSpecies.IsLegendary
            }
        }
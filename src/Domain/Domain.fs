//////
/// Module defining the types used throughout the application
//////

module Domain.Types

type PokemonDetails = {
    Id: int
    Name: string
}

type Pokemon = {
    Name: string
    Description: string
    Habitat: string
    IsLegendary: bool
}
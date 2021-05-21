//////
/// Module defining the types used throughout the application
//////

module Core.Domain

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
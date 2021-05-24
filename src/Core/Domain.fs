//////
/// Module defining the types used throughout the application
//////

module Core.Domain

type Pokemon = {
    Name: string
    Description: string
    Habitat: string
    IsLegendary: bool
}

/// Discriminated union to distinguish the traduction types
type Translation = 
    | Yoda 
    | Shakespeare

namespace UtilitiesTests

open System
open Xunit
open FsUnit.Xunit
open FsCheck.Xunit

open Core.Utilities
open Core.JsonProviders
open FsCheck
open System.Collections.Generic


module pathConcatanationOperator = 
    [<Fact>]
    let ``Adds forward slash between lhs and rhs`` () =
        "lhs" +/ "rhs" |> should equal "lhs/rhs"

    [<Fact>]
    let ``Removes repeated slashes`` () =
        let expected = "lhs/rhs"
        "lhs/" +/ "rhs" |> should equal expected
        "lhs/" +/ "/rhs" |> should equal expected
        "lhs\\" +/ "rhs" |> should equal expected
        "lhs\\" +/ "/rhs" |> should equal expected
        "lhs\\" +/ "\\rhs" |> should equal expected
        "lhs" +/ "\\rhs" |> should equal expected
        "lhs/" +/ "\\rhs" |> should equal expected

    [<Fact>]
    let ``Handles ints as rhs`` () =
        "lhs" +/ 123 |> should equal "lhs/123"


module filterOutEscapeCharacters = 
    /// Property-based test using FsCheck: https://fscheck.github.io/FsCheck/
    /// The framework generates 100 "random" inputs, to validate if the property holds
    [<Property>]
    let ``Returns input if it has no escape characters`` (input: NonEmptyString) =
        let inputString = input.Get

        /// The property is evaluated only if \n and \f are NOT present in the input
        not (inputString.Contains('\n') || inputString.Contains('\f')) 
        ==> (filterOutEscapeCharacters inputString = inputString)

    [<Fact>]
    let ``Returns input if empty`` () =
        filterOutEscapeCharacters String.Empty |> should equal String.Empty

    [<Fact>]
    let ``Returns emptry string if input null`` () =
        filterOutEscapeCharacters null |> should equal String.Empty

    [<Fact>]
    let ``Replaces \n by a space`` () =
        filterOutEscapeCharacters "some\nstring\nwith\nline\nbreak" |> should equal "some string with line break"

    [<Fact>]
    let ``Replaces \f by a space`` () =
        filterOutEscapeCharacters "some\fstring\fwith\ffeed" |> should equal "some string with feed"


module makePokemon = 
    [<Fact>]
    let ``Builds a Pokemon object from a raw response of the pokeapi`` () =
        let testSample = """
        {
        "name": "some name",
        "flavor_text_entries": [
          {
            "flavor_text": "some description\nwithout\nescape\fchars",
            "language": {
              "name": "en"
            }
          }
        ],
        "habitat": {
          "name": "some habitat"
        },
        "is_legendary": false
        }"""
        let rawPokemonSpecies = PokemonSpeciesProvider.Parse(testSample)
        let log x = x |> ignore //dummy mock logger
        let pokemon = makePokemon log rawPokemonSpecies

        pokemon.Name |> should equal "some name"
        pokemon.Description |> should equal "some description without escape chars"
        pokemon.Habitat |> should equal "some habitat"
        pokemon.IsLegendary |> should be False

    [<Fact>]
    let ``Throws an exception if something went wrong`` () =
        //No english language name in the FlavorTextEntries
        let testSample = """
        {
        "name": "some name"
        }"""
        let rawPokemonSpecies = PokemonSpeciesProvider.Parse(testSample)
        let log x = x |> ignore //dummy mock logger
        (fun() -> makePokemon log rawPokemonSpecies |> ignore) 
        |> should throw typeof<KeyNotFoundException>

    



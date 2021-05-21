namespace UtilitiesTests

open System
open Xunit
open FsUnit.Xunit
open FsCheck.Xunit

open Core.Utilities
open FsCheck

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
    let ``Returrns emptry string if null`` () =
        filterOutEscapeCharacters null |> should equal String.Empty

    [<Fact>]
    let ``Replaces \n by a space`` () =
        filterOutEscapeCharacters "some\nstring\nwith\nline\nbreak" |> should equal "some string with line break"

    [<Fact>]
    let ``Replaces \f by a space`` () =
        filterOutEscapeCharacters "some\fstring\fwith\ffeed" |> should equal "some string with feed"



    



module UtilitiesTests

open System
open Xunit
open FsUnit.Xunit
open FsCheck.Xunit

open APIs.Utilities

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


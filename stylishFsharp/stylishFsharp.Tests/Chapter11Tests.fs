module Chapter11Tests

open Swensen.Unquote
open Xunit

open Chapter11

let isFailure = function
    | Success _ -> 1 <>! 1
    | Failure _ -> 1 =! 1

let isError = function
    | Ok _ -> 1 <>! 1
    | Error _ -> 1 =! 1
    
[<Fact>]
let ``Exercise 11-1 - Reproducing MapError`` () =
    "ups" |> Failure |> passThroughRejects id |> isFailure
    
    // the same using built-in "mapError":
    "foo" |> Error |> Result.mapError id |> isError

module Chapter11Tests

open Swensen.Unquote
open Xunit

open Chapter11.Exercise11_01

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

open System
open Chapter11.Exercise11_02

[<Fact>]
let ``Exercise 11-2 - Writing a ROP Pipeline`` () =
    
    let example =
        [|
            { FileName = "2019-02-23T02:00:00:00-05:00"
              Content = [|1.0; 2.0; 3.0; 4.0|] }
            { FileName = "2019-02-23T02:00:00:10-05:00"
              Content = [|5.0; 6.0; 7.0; 8.0|] }
            { FileName = "error"
              Content = [||] }
            { FileName = "2019-02-23T02:00:00:20-05:00"
              Content = [|1.0; 2.0; 3.0; Double.NaN|] }
        |]

//    example
//    |> processData
//    |> Array.iter (printfn "%A")
    0
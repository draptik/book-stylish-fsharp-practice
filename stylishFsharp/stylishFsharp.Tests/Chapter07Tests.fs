module Chapter07Tests

open System
open System.Diagnostics
open Xunit
open Swensen.Unquote

open Chapter07

[<Fact(Skip="doesn't always work...")>]
let ``Exercise 7-1: Records and Performance Is Struct faster?`` () =
    let stopWatch = Stopwatch.StartNew()
    let result1 =
        Array.init 10_000_000 (fun x ->
            { X = float32 x; Y = float32 x; Z = float32 x; Time = DateTime.Now })
    stopWatch.Stop()
    let t1 = stopWatch.ElapsedMilliseconds

    stopWatch.Restart()
    let result2 =
        Array.init 10_000_000 (fun x ->
            { PositionStruct.X = float32 x; Y = float32 x; Z = float32 x; Time = DateTime.Now })
    stopWatch.Stop()
    let t2 = stopWatch.ElapsedMilliseconds

    test <@ t1 > t2 @>

[<Fact>]
let ``Exercise 7-3: Equality and Comparison`` () =
    (*
    let tracks =
        [ Track("The Mollusk", "Ween")
          Track("Bread Hair", "They Might Be Giants")
          Track("The Mollusk", "Ween") ]
    *)
    let tracks = [
        { Name = "The Mollusk"; Artist = "Ween" }
        { Name = "Bread Hair"; Artist = "They Might Be Giants" }
        { Name = "The Mollusk"; Artist = "Ween" }
    ]
    let actual = tracks |> Set.ofList
    
    let expected = Set[
       { Name = "Bread Hair"; Artist = "They Might Be Giants" }
       { Name = "The Mollusk"; Artist = "Ween" }]
    
    test <@ actual = expected @>
            
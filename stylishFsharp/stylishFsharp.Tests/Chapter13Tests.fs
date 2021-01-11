module Chapter13Tests

open System.IO
open Swensen.Unquote
open Xunit
open Xunit.Abstractions

open Chapter13

type Chapter13WithOutput(o : ITestOutputHelper) =
    let output = o
    let log msg = output.WriteLine (sprintf "%s" msg)

    [<Fact(Skip="TODO")>]
    let ``todo`` () =
        log (sprintf "%s" "test")
        let demo() =
            // Brightest 10 minor planets (absolute magnitude)
            Directory.GetCurrentDirectory() + @"../../../../sampledata/MPCORB.DAT"
            |> File.ReadLines
            |> MinorPlanets.createFromData
            |> Seq.filter (fun mp ->
                mp.AbsMag |> Option.isSome)
            |> Seq.sortBy (fun mp ->
                mp.AbsMag.Value)
            |> Seq.truncate 10
            |> Seq.map (fun mp ->
                log (sprintf "Name: %s Abs. magnitude: %0.2f"
                    mp.ReadableDesignation
                    (mp.AbsMag |> Option.defaultValue nan)))
        demo()
        1 =! 1
        

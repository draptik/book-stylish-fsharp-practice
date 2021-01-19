module Chapter13Tests

open System.IO
open Swensen.Unquote
open Xunit
open Xunit.Abstractions

open Chapter13

type Chapter13WithOutput(o : ITestOutputHelper) =
    let output = o
    let log msg = output.WriteLine (sprintf "%s" msg)
    
    // NOTE this test takes ~20sec in Rider :-(
    [<Fact>]
    let ``Initial test of demo function`` () =

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
                (sprintf "Name: %s Abs. magnitude: %0.2f"
                    mp.ReadableDesignation
                    (mp.AbsMag |> Option.defaultValue nan)))
            |> List.ofSeq // converting to list because seq can't be compared in test
        
        let actual = demo()
        actual |> List.iter (log) // debugging

        let expected = [
            "Name: (136199) Eris Abs. magnitude: -1.10"
            "Name: (134340) Pluto Abs. magnitude: -0.40"
            "Name: (136472) Makemake Abs. magnitude: -0.10"
            "Name: (136108) Haumea Abs. magnitude: 0.30"
            "Name: (90377) Sedna Abs. magnitude: 1.30"
            "Name: (225088) Gonggong Abs. magnitude: 1.90"
            "Name: (90482) Orcus Abs. magnitude: 2.30"
            "Name: (50000) Quaoar Abs. magnitude: 2.50"
            "Name: (4) Vesta Abs. magnitude: 3.20"
            "Name: (532037) 2013 FY27 Abs. magnitude: 3.20"
        ]
        actual =! expected

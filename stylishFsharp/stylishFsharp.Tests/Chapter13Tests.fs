module Chapter13Tests

open System.IO
open Swensen.Unquote
open Xunit
open Xunit.Abstractions

open Chapter13
open Exercise13_1

type Chapter13WithOutput(o : ITestOutputHelper) =
    let output = o
    let log msg = output.WriteLine (sprintf "%s" msg)
    
    [<Fact(Skip="long running test (~20sec)")>]
    let ``Initial test of demo function`` () =

        let demo() =
            // Brightest 10 minor planets (absolute magnitude)
            Directory.GetCurrentDirectory() + @"../../../../sampledata/MPCORB.DAT"
            |> File.ReadLines
            |> MinorPlanet.fromMpcOrbData
            |> Seq.filter (fun mp ->
                mp.H |> Option.isSome)
            |> Seq.sortBy (fun mp ->
                mp.H.Value)
            |> Seq.truncate 10
            |> Seq.map (fun mp ->
                (sprintf "Name: %s Abs. magnitude: %0.2f"
                    mp.ReadableDesignation
                    (mp.H |> Option.defaultValue nan)))
            |> List.ofSeq // converting to list because seq can't be compared in test
        
        let actual = demo()
        actual |> List.iter (log) // debugging

        let expected = [
             "Name: (136199) Eris Abs. magnitude: -1.10"
             "Name: (134340) Pluto Abs. magnitude: -0.45"
             "Name: (136472) Makemake Abs. magnitude: -0.13"
             "Name: (136108) Haumea Abs. magnitude: 0.30"
             "Name: (90377) Sedna Abs. magnitude: 1.30"
             "Name: (225088) Gonggong Abs. magnitude: 1.93"
             "Name: (90482) Orcus Abs. magnitude: 2.31"
             "Name: (50000) Quaoar Abs. magnitude: 2.51"
             "Name: (532037) 2013 FY27 Abs. magnitude: 3.20"
             "Name: (4) Vesta Abs. magnitude: 3.28" ]
                    
        actual =! expected

    let setup names = names |> Seq.iter Helper.createReadOnlyFile
    let cleanup names = names |> Seq.iter Helper.deleteFile
    
    [<Fact>]
    let ``Exercise 13-1 - Making code readable - initial code`` () =
        let testFileNames = seq { "test_1.txt"; "test_2.txt"; "test_3_abc.txt"; "test_4_abc.txt" }
        testFileNames |> setup        
        
        let pattern = "test.*abc\.txt"
        let currentDir = "."
        
        let filenames = InitialCode.find pattern currentDir
        
        filenames |> Seq.iter log
        (filenames |> Seq.length) =! 2

        testFileNames |> cleanup
        
    [<Fact>]
    let ``Exercise 13-1 - Making code readable - improved code`` () =
        let testFileNames = seq { "test_1.txt"; "test_2.txt"; "test_3_abc.txt"; "test_4_abc.txt" }
        testFileNames |> setup        
        
        let pattern = "test.*abc\.txt"
        let currentDir = "."
        
        let filenames = ImprovedCode.find pattern currentDir
        
        filenames |> Seq.iter log
        (filenames |> Seq.length) =! 2

        testFileNames |> cleanup
module stylishFsharp.Console.Chapter12

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

module InappropriateCollectionType =
    module Old =
        
        /// Listing 12-5
        let sample interval data =
            [
                let max = (List.length data) - 1
                for i in 0..interval..max ->
                    data.[i]
            ]

    module New =
        /// Listing 12-5
        //        let sample interval data =
        //            [
        //                let max = (List.length data) - 1
        //                for i in 0..interval..max ->
        //                    data.[i]
        //            ]

        /// Listing 12-8
        //        let sample interval data =
        //            data
        //            |> List.indexed
        //            |> List.filter (fun (i, _) ->
        //                i % interval = 0)
        //            |> List.map snd
        //        (*
        //            | Method |       Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
        //            |------- |-----------:|---------:|---------:|----------:|----------:|----------:|------------:|
        //            |    Old | 1,325.1 ms | 24.79 ms | 21.97 ms |         - |         - |         - |    33.91 KB |
        //            |    New |   135.1 ms |  2.67 ms |  6.75 ms | 8750.0000 | 5000.0000 | 1250.0000 | 62562.99 KB |
        //        
        //            - Pros: 10x faster
        //            - Cons: a lot of garbage collection going on (we have 3 lists now!)
        //        *)

        /// Listing 12-10                
        let sample interval data =
            data
            |> Array.indexed
            |> Array.filter (fun (i, _) ->
                i % interval = 0)
            |> Array.map snd
        (*
            | Method |        Mean |     Error |    StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
            |------- |------------:|----------:|----------:|----------:|----------:|---------:|------------:|
            |    Old | 1,320.65 ms | 13.895 ms | 12.997 ms |         - |         - |        - |    33.91 KB |
            |    New |    73.67 ms |  1.065 ms |  0.996 ms | 4428.5714 | 2571.4286 | 714.2857 | 39201.71 KB |
            
            - Using Array instead of List
            - same Pros and Cons as before
        *)
            
module Harness =
    
    [<MemoryDiagnoser>]
    type Harness() =

        let r = Random()
        let list = List.init 1_000_000 (fun _ -> r.NextDouble())
        let array = list |> Array.ofList
        
        [<Benchmark>]
        member __.Old() =
            list
            |> InappropriateCollectionType.Old.sample 1000
            |> ignore
        
        [<Benchmark>]
        member __.New() =
            array
            |> InappropriateCollectionType.New.sample 1000
            |> ignore

let runChapter12 () =
    BenchmarkRunner.Run<Harness.Harness>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
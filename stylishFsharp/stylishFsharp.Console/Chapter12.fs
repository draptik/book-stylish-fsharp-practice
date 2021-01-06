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
        let sample interval data =
            data
            |> List.indexed
            |> List.filter (fun (i, _) ->
                i % interval = 0)
            |> List.map snd
        (*
            | Method |       Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
            |------- |-----------:|---------:|---------:|----------:|----------:|----------:|------------:|
            |    Old | 1,325.1 ms | 24.79 ms | 21.97 ms |         - |         - |         - |    33.91 KB |
            |    New |   135.1 ms |  2.67 ms |  6.75 ms | 8750.0000 | 5000.0000 | 1250.0000 | 62562.99 KB |
        
            - Pros: 10x faster
            - Cons: a lot of garbage collection going on (we have 3 lists now!)
        *)
module Harness =
    
    [<MemoryDiagnoser>]
    type Harness() =

        let r = Random()
        let list = List.init 1_000_000 (fun _ -> r.NextDouble())
        
        [<Benchmark>]
        member __.Old() =
            list
            |> InappropriateCollectionType.Old.sample 1000
            |> ignore
        
        [<Benchmark>]
        member __.New() =
            list
            |> InappropriateCollectionType.New.sample 1000
            |> ignore

let runChapter12 () =
    BenchmarkRunner.Run<Harness.Harness>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
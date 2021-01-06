module stylishFsharp.Console.Chapter12

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

module InappropriateCollectionType =
    module Old =
        let sample interval data =
            [
                let max = (List.length data) - 1
                for i in 0..interval..max ->
                    data.[i]
            ]

    module New =
        let sample interval data =
            [
                let max = (List.length data) - 1
                for i in 0..interval..max ->
                    data.[i]
            ]

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
            |> InappropriateCollectionType.Old.sample 1000
            |> ignore

let runChapter12 () =
    BenchmarkRunner.Run<Harness.Harness>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
module stylishFsharp.Console.Chapter12

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Chapter12


module Harness =
    
    [<MemoryDiagnoser>]
    type Harness() =
    
        [<Benchmark>]
        member __.Old() =
            Dummy.slowFunction()
        
        [<Benchmark>]
        member __.New() =
            Dummy.fastFunction()

let runChapter12 () =
    BenchmarkRunner.Run<Harness.Harness>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
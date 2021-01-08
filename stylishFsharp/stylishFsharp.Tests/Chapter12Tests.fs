module Chapter12Tests

open System
open System.Diagnostics
open BenchmarkDotNet.Running
open BenchmarkDotNet.Attributes
open Swensen.Unquote
open Xunit
open Chapter12
open Xunit.Abstractions
open Chapter12.Exercise12_1

module Harness =
    
    [<MemoryDiagnoser>]
    type Harness() =
    
        [<Benchmark>]
        member __.Old() =
            Dummy.slowFunction()
        
        [<Benchmark>]
        member __.New() =
            Dummy.fastFunction()

type Chapter12TestsWithOutput(o : ITestOutputHelper) =
    let output = o
    let log msg = output.WriteLine (sprintf "%s" msg)
        
    /// Before running this test the `stylishFsharp` project settings must be set to `Release`        
    [<Fact(Skip="TODO Figure out how to use BenchmarkDotNet in unit tests without having to recompile in Release mode")>]
    let ``Benchmark test harness works`` () =
        let summary = BenchmarkRunner.Run<Harness.Harness>()
        log (sprintf "Summary: %A" summary.Reports)
        let old = summary.Reports.[0].ResultStatistics.Mean
        let new' = summary.Reports.[1].ResultStatistics.Mean
        old >! new'


    let measure f a b =
        let sw = Stopwatch.StartNew()
        let iterations = 100_000_000
        for i in 0..iterations do
            f a b |> ignore
        sw.Elapsed.Milliseconds

    // NOTE: This test is really slow when run using Rider's test runner (~30sec). When executed
    // from the cli using `dotnet test` it is executed in ~1sec.
    [<Fact(Skip="Performance testing in unit testing is not very reliable")>]
    let ``Exercise 12-1 - Concatenating collections`` () =
            
        let a = [{Id = 1}]
        let b = [{Id = 2}]
        let a' = a |> Array.ofList
        let b' = b |> Array.ofList
        
        let t1 = measure addTransactionsBaseline a b
        let t2 = measure addTransactions a' b'
        
        log (sprintf "t1: %ims; t2: %ims" t1 t2)
        
        t1 >! t2
    
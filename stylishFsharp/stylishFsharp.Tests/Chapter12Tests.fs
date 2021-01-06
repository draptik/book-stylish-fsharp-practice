module Chapter12Tests

open System
open System.Diagnostics
open BenchmarkDotNet.Running
open BenchmarkDotNet.Attributes
open Swensen.Unquote
open Xunit
open Chapter12
open Xunit.Abstractions

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

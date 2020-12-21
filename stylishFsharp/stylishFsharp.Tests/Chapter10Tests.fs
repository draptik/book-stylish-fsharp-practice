module Chapter10Tests

open System
open System.Diagnostics
open Xunit
open Swensen.Unquote
open Chapter10
open Xunit.Abstractions

(* This is just an experiment to see if we can capture side-effects in xunit... *)
type Chapter10TestsWithOutput(o : ITestOutputHelper) =
    let output = o 

    [<Fact>]
    let ``Output sample`` () =
        output.WriteLine("Hello from test")
    
[<Fact(Skip="Sample code from book")>]
let ``Demo 1 - Synchronous code`` () =
    let uri = Uri @"https://minorplanetcenter.net/data"
    let pattern = @"neam.*\.json\.gz$"
    
    let localPath = @"~/tmp/fsharp-downloads"
    
    let sw = Stopwatch.StartNew()
    
    let downloaded, failed =
        Download.GetFiles uri pattern localPath
    sw.Stop()
    
    failed
    |> Array.iter (fun fn ->
        Log.report ConsoleColor.Red (sprintf "Failed: %s" fn))
    
    Log.cyan (sprintf "%i files downloaded in %0.1fs, %i failed. Press any key" downloaded.Length sw.Elapsed.TotalSeconds failed.Length)
    
//    Console.ReadKey() |> ignore
    0

[<Fact(Skip="Slow running test")>]
let ``Demo 2 - Synchronous code`` () =
    let stopwatch = Stopwatch.StartNew()
    let actualDownloaded, actualFailed = Run.Get
    stopwatch.Stop()
    
    test <@ actualDownloaded.Length = 16 @>
    test <@ actualFailed.Length = 0 @>
    test <@ stopwatch.Elapsed.TotalSeconds > 10. @>
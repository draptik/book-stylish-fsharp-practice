module Chapter10Tests

open System
open System.Diagnostics
open System.Threading

open Swensen.Unquote
open Xunit
open Xunit.Abstractions

open Chapter10

(* This is just an experiment to see if we can capture side-effects in xunit using ITestOutputHelper... *)
type Chapter10TestsWithOutput(o : ITestOutputHelper) =
    let output = o 

    let logReport message =
        output.WriteLine (sprintf "%s (thread ID: %i)" message Thread.CurrentThread.ManagedThreadId)
    
    [<Fact>]
    let ``Output sample`` () =
        output.WriteLine("Hello from test")
    
    [<Fact(Skip="Sample code from book, slightly adopted for testing; Slow running test - only include manually")>]
//    [<Fact>]
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
            logReport (sprintf "Failed: %s" fn))
        
        logReport (sprintf "%i files downloaded in %0.1fs, %i failed. Press any key" downloaded.Length sw.Elapsed.TotalSeconds failed.Length)

    [<Fact(Skip="Slow running test - only include manually")>]
//    [<Fact>]
    let ``Demo 2 - Synchronous code`` () =
        let { Outcomes = actualDownloaded, actualFailed; ElapsedSeconds = elapsedSeconds } =
            Run.GetAll

        logReport (sprintf "Failed downloads: %i" actualFailed.Length)
        logReport (sprintf "Successful downloads: %i" actualDownloaded.Length)
        logReport (sprintf "Elapsed Seconds: %f" elapsedSeconds)

        test <@ actualDownloaded.Length = 16 @>
        test <@ actualFailed.Length = 0 @>
        test <@ elapsedSeconds > 1. @>

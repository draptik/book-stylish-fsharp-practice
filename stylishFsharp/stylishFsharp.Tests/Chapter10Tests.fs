module Chapter10Tests

open System
open System.Diagnostics
open Swensen.Unquote
open Xunit
open Xunit.Abstractions

open Chapter10

type Chapter10TestsWithOutput(o : ITestOutputHelper) =
    let output = o 

    let logReport message = output.WriteLine (sprintf "%s" message)
    
    let numberOfThreads outcomes =
        outcomes
            |> Array.filter Outcome.isOk
            |> Array.map Outcome.threadId
            |> Array.distinct
            |> Array.length
            
    [<Fact(Skip="Usage example of output.WriteLine")>]
    let ``Output sample`` () = output.WriteLine("Hello from test")
    
    [<Fact(Skip="Sample code from book, slightly adopted for testing; Slow running test - only include manually")>]
//    [<Fact>]
    let ``Demo 1 - Sync code`` () =
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

//    [<Fact(Skip="Slow running test - only include manually")>]
    [<Fact>]
    let ``Demo 2 - Sync code`` () =
        let { Outcomes = actualDownloaded, actualFailed; ElapsedSeconds = elapsedSeconds } =
            Run.GetAll // <- sync!

        logReport (sprintf "Failed downloads: %i" actualFailed.Length)
        logReport (sprintf "Successful downloads: %i" actualDownloaded.Length)
        logReport (sprintf "Elapsed Seconds: %fs" elapsedSeconds)

        actualDownloaded
        |> Array.iter (fun x -> logReport (sprintf "%A" x))

        actualDownloaded.Length =! 16
        actualFailed.Length =! 0
        elapsedSeconds >! 1.
        numberOfThreads actualDownloaded =! 1 // <- sync code only uses single thread


//    [<Fact(Skip="Slow running test - only include manually")>]
    [<Fact>]
    let ``Demo 3 - Async code`` () =
        let { Outcomes = actualDownloaded, actualFailed; ElapsedSeconds = elapsedSeconds } =
            Run.GetAllAsync // <- async!
            |> Async.RunSynchronously

        logReport (sprintf "Failed downloads: %i" actualFailed.Length)
        logReport (sprintf "Successful downloads: %i" actualDownloaded.Length)
        logReport (sprintf "Elapsed Seconds: %fs" elapsedSeconds)

        actualDownloaded
        |> Array.iter (fun x -> logReport (sprintf "%A" x))

        actualDownloaded.Length =! 16
        actualFailed.Length =! 0
        elapsedSeconds >! 1.
        numberOfThreads actualDownloaded >! 1 // <- async code uses multiple threads

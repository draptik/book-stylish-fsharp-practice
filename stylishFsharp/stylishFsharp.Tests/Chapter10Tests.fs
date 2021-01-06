module Chapter10Tests

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

    [<Fact(Skip="Slow running test - only include manually")>]
//    [<Fact>]
    let ``Async code with throttle`` () =
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

    [<Fact>]
    let ``Exercise 10-1: Making some code asynchronous 1. sync example`` () =
        let stopWatch = Stopwatch.StartNew()
        let result = Exercise10_01.Consumer.GetData 10
        result
        |> Array.iter (fun x -> logReport(sprintf "%s" x))
        stopWatch.ElapsedMilliseconds >! 5000L

//    [<Fact>]
//    let ``Exercise 10-1: Making some code asynchronous 2. async solution`` () =
//        let stopWatch = Stopwatch.StartNew()
//        let result =
//            async {
//                let! s = Exercise10_01.Consumer.AsyncGetData 10
//                return s
//            }
//        
//        result
//        |> Array.iter (fun x -> logReport(sprintf "%s" x))
//        stopWatch.ElapsedMilliseconds >! 5000L

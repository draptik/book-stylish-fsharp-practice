module Chapter10Tests

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

//    [<Fact(Skip="Slow running test - only include manually")>]
    [<Fact>]
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

module Chapter10

open System.Diagnostics
open System.IO
open System.Threading

module Log =
    open System
    
    let report (color : ConsoleColor) (message : string) =
        Console.ForegroundColor <- color
        printfn "%s (thread ID: %i)" message Thread.CurrentThread.ManagedThreadId 
        Console.ResetColor()
        
    let red = report ConsoleColor.Red
    let green = report ConsoleColor.Green
    let yellow = report ConsoleColor.Yellow
    let cyan = report ConsoleColor.Cyan
    
    
(* NOTE: Outcome differs from book example: Outcome contains tuple filename*message *)    
module Outcome =
    type Outcome =
        | OK of filename:string * msg:string * threadId:int
        | Failed of filename:string * msg:string * threadId:int
        
    let isOk = function
        | OK _ -> true
        | Failed _ -> false
    
    let fileName = function
        | OK (fn, _, _) -> fn
        | Failed (fn, _, _) -> fn
        
    let threadId = function
        | OK (_, _, t) -> t
        | Failed (_, _, t) -> t

module Download =
    open System
    open System.Net
    open System.Text.RegularExpressions
    // NuGet package FSharp.Data
    open FSharp.Data
    // From Nuget package "FSharpx.Async"
    open FSharpx.Control

    let private absoluteUri (pageUri : Uri) (filePath : string) =
        if filePath.StartsWith("http:") || filePath.StartsWith("https:") then
            Uri(filePath)
        else
            let sep = '/'
            filePath.TrimStart(sep)
            |> (sprintf "%O%c%s" pageUri sep)
            |> Uri
 
    let private getLinks (pageUri : Uri) (filePattern : string) =
        async {
            Log.cyan "Getting names..."
            let re = Regex(filePattern)
            let! html = HtmlDocument.AsyncLoad(pageUri.AbsoluteUri)
            
            let links =
                html.Descendants ["a"]
                |> Seq.choose (fun node ->
                    node.TryGetAttribute("href")
                    |> Option.map (fun att -> att.Value()))
                |> Seq.filter (re.IsMatch)
                |> Seq.map (absoluteUri pageUri)
                |> Seq.distinct
                |> Array.ofSeq
                
            return links
        }

    let private tryDownload (localPath : string) (fileUri : Uri) =
        async {
            let fileName = fileUri.Segments |> Array.last
            Log.yellow (sprintf "%s - starting download" fileName)
            let filePath = Path.Combine(localPath, fileName)
            
            use client = new WebClient()
            try
                do!
                    client.DownloadFileTaskAsync(fileUri, filePath)
                    |> Async.AwaitTask
                Log.green (sprintf "%s - download complete" fileName)
                return Outcome.OK (fileName, (sprintf "%s - download complete" fileName), Thread.CurrentThread.ManagedThreadId)
            with
            | e ->
                Log.red (sprintf "%s - error: %s" fileName e.Message)
                return Outcome.Failed (fileName, (sprintf "%s - error: %s" fileName e.Message), Thread.CurrentThread.ManagedThreadId)
        }

    let AsyncGetOutcomes (pageUri : Uri) (filePattern : string) (localPath : string) (throttle : int) =
        async {
            let! links = getLinks pageUri filePattern
            let! downloadResults =
                links
                |> Seq.map (tryDownload localPath)
                |> Async.ParallelWithThrottle throttle
            let downloaded, failed =
                downloadResults
                |> Array.partition Outcome.isOk
            return downloaded, failed
        }

type OutcomeResult = {
    Outcomes: Outcome.Outcome[] * Outcome.Outcome[]
    ElapsedSeconds: float
}

module Run =
    open System

    /// small dataset: minor planets dataset    
    let uri = Uri @"https://minorplanetcenter.net/data"
    let pattern = @"neam.*\.json\.gz$"

    /// large dataset (13GB): google n-grams dataset    
    //    let uri = Uri @"https://storage.googleapis.com/books/ngrams/books/datasetsv2.html"
    //    let pattern = @"eng\-1M\-2gram.*\.zip$"
    let localPath = ""
                   
    let GetAllAsync =
        async {
            let stopwatch = Stopwatch.StartNew()
            let! outcomes = Download.AsyncGetOutcomes uri pattern localPath 4
            stopwatch.Stop()

            let elapsedSeconds = stopwatch.Elapsed.TotalSeconds
            
            return { Outcomes = outcomes; ElapsedSeconds = elapsedSeconds }
        }

module Exercises =
    open System
    
    module Random =
        let private random = System.Random()
        let string () =
            let len = random.Next(0, 10)
            Array.init len (fun _ -> random.Next(0, 255) |> char)
            |> String
            
    module Server =
        let AsyncGetString (id : int) =
            // id is unused
            async {
                do! Async.Sleep(500)
                return Random.string()
            }
            
    module Consumer =
        let GetData (count : int) =
            let strings =
                Array.init count (fun i ->
                    Server.AsyncGetString i
                    |> Async.RunSynchronously)
            strings
            |> Array.sort

        let AsyncGetData (count : int) =
            async {
                let! strings =
                    Array.init count (fun i ->
                        Server.AsyncGetString i) |> Async.Parallel
                return
                    strings
                    |> Array.sort
            }

        /// Returns a C# Task (instead of an F# Async)
        let GetDataAsync (count : int) =
            count
            |> AsyncGetData
            |> Async.StartAsTask

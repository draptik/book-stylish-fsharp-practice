module stylishFsharp.Console.Chapter10

module Log =
    open System
    open System.Threading
    
    /// Print a colored log message
    let report =
        let lockObj = obj()
        fun (color : ConsoleColor) (message : string) ->
            lock lockObj (fun _ ->
                Console.ForegroundColor <- color
                printfn "%s (thread ID: %i)" message Thread.CurrentThread.ManagedThreadId
                Console.ResetColor())
        
    let red = report ConsoleColor.Red
    let green = report ConsoleColor.Green
    let yellow = report ConsoleColor.Yellow
    let cyan = report ConsoleColor.Cyan

module Outcome =
    type Outcome =
        | OK of filename:string
        | Failed of filename:string
    
    let isOk = function
        | OK _ -> true
        | Failed _ -> false
    
    let filename = function
        | OK fn -> fn
        | Failed fn -> fn

module Download =
    open System
    open System.IO
    open System.Net
    open System.Text.RegularExpressions
    // From Nuget package "FSharp.Data"
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
    
    /// Get the URLs of all links in a specified page matching a specified regex pattern.
    let private getLinks (pageUri : Uri) (filePattern : string) =
        async {
            Log.cyan "Getting names..."
            let re = Regex(filePattern)
            
            // val html : Async<HtmlDocument>
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
        
    /// Download a file to the specified local path
    let private tryDownload (localPath : string) (fileUri : Uri) =
        async {
            let fileName = fileUri.Segments |> Array.last
            Log.yellow (sprintf "%s - starting download" fileName)
            let filePath = Path.Combine(localPath, fileName)
            
            use client = new WebClient()
            try
                do! client.DownloadFileTaskAsync(fileUri, filePath) |> Async.AwaitTask
                Log.green (sprintf "%s - download complete" fileName)
                return Outcome.OK fileName
            with
            | e ->
                Log.red (sprintf "%s - error: %s" fileName e.Message)
                return Outcome.Failed fileName
        }
            
    /// Download all the files linked to in the specified webpage, whose
    /// link path matches the specified regular expression, to the specified
    /// local path. Return a tuple of succeeded and failed file names.
    let AsyncGetFilesThrottled (pageUri : Uri) (filePattern : string) (localPath : string) (throttle : int) =
        async {
            let! links = getLinks pageUri filePattern
            
            let! downloadResults =
                links
                |> Seq.map (tryDownload localPath)
                |> Async.ParallelWithThrottle throttle
                
            let downloaded, failed =
                downloadResults
                |> Array.partition Outcome.isOk

            return                
                downloaded |> Array.map Outcome.filename,
                failed |> Array.map Outcome.filename
        }

open System
open System.Diagnostics

let runChapter10() =
    // Some minor planets data:
    let uri = Uri @"https://minorplanetcenter.net/data"
    let pattern = @"neam.*\.json\.gz$"
    
    // Large dataset (~13GB)
    //    let uri = Uri @"https://storage.googleapis.com/books/ngrams/books/datasetsv2.html"
    //    let pattern = @"eng\-1M\-2gram.*\.zip$"
    
    let localPath = "/home/patrick/tmp/downloads"
    
    let sw = Stopwatch()
    sw.Start()
    
    let downloaded, failed =
        Download.AsyncGetFilesThrottled uri pattern localPath 4
        |> Async.RunSynchronously
        
    failed
    |> Array.iter (fun fn ->
        Log.report ConsoleColor.Red (sprintf "Failed: %s" fn))
    
    Log.cyan (sprintf "%i files downloaded in %0.1fs, %i failed. Press a key"
                  downloaded.Length sw.Elapsed.TotalSeconds failed.Length)
    
    Console.ReadKey() |> ignore
    
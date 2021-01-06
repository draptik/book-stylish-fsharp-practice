module Chapter11

module Exercise11_01 =
    
    type Outcome<'TSuccess, 'TFailure> =
        | Success of 'TSuccess
        | Failure of 'TFailure

    // "bind"
    let adapt func input =
        match input with
        | Success x -> func x
        | Failure f -> Failure f

    // "map"
    let passThrough func input =
        match input with
        | Success x -> func x |> Success
        | Failure f -> Failure f

    // "mapError"
    let passThroughRejects func input =
        match input with
        | Success x -> Success x
        | Failure f -> func f |> Failure

module Exercise11_02 =
    open System
    
    type Message = {
        FileName : string
        Content : float[]
    }
        
    type Reading = {
        TimeStamp : DateTimeOffset
        Data : float[]
    }
    
    let log s = sprintf "Logging: %s" s
    
    type MessageError =
        | InvalidFileName of fileName:string
        | DataContainsNaN of fileName:string * index:int
    
    let getReading message =
        match DateTimeOffset.TryParse(message.FileName) with
        | true, dt ->
            let reading = { TimeStamp = dt; Data = message.Content }
            Ok (message.FileName, reading)
        | false, _ ->
            Error (InvalidFileName message.FileName)
            
    let validateData(fileName, reading) =
        let nanIndex =
            reading.Data
            |> Array.tryFindIndex (Double.IsNaN)
        match nanIndex with
        | Some i ->
            Error (DataContainsNaN (fileName, i))
        | None ->
            Ok reading
        
    let logError (e: MessageError) =
        match e with
        | InvalidFileName fn -> log fn
        | DataContainsNaN (fn, i) -> log (sprintf "%s %i" fn i)
    
    open Result
    
    let processMessage =
        getReading
        >> bind validateData
        >> mapError logError
        
    let processData data =
        data
        |> Array.map processMessage
        |> Array.choose (fun result ->
            match result with
            | Ok reading -> reading |> Some
            | Error _ -> None)

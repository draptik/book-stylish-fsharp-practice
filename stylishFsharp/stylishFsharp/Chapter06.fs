module Chapter06

open System

type MeterValue =
    | Standard of int
    | Economy of Day:int * Night:int // <- using tuple-label syntax

type MeterReading = {
    ReadingDate : DateTime
    MeterValue : MeterValue
}

// Pattern matching on record
let formatReading { ReadingDate = date; MeterValue = meterValue } =
    let dateString = date.ToString("yyyy-MM-dd")    
    match meterValue with
    | Standard reading ->
        sprintf "Your reading on: %s was %07i" dateString reading
    | Economy(Day=day; Night=night) ->
        sprintf "Your reading on: %s: Day: %07i Night: %07i" dateString day night


let getFruits fruits =
    fruits
    |> List.map (fun (name, count) ->
        sprintf "There are %i %s" count name)

type FruitBatch = {
    Count : int
    Name : string
}

let fruitListToFruitBatchList fruits =
    fruits
    |> List.map (fun (name, count) ->
        {Name = name; Count = count})

let getFruitsFromBatches fruitBatches =
    fruitBatches
    // pattern matching on record
    |> List.map (fun {Name = name; Count = count} ->
        sprintf "There are %i %s" count name)

open System.Text.RegularExpressions

let (|USZipCode|_|) s =
    let m = Regex.Match(s, @"^(\d{5})$")
    if m.Success then
        USZipCode s |> Some
    else
        None

let (|USZipPlus4Code|_|) s =
    let m = Regex.Match(s, @"^(\d{5})\-(\d{4})$")
    if m.Success then
        USZipPlus4Code (m.Groups.[0].Value,
                        m.Groups.[1].Value)
        |> Some
    else
        None

let checkZipcodes zipCodes =
    zipCodes
    |> List.map (fun z ->
        match z with
        | USZipCode c ->
            sprintf "A normal zip code: %s" c
        | USZipPlus4Code (code, suffix) ->
            sprintf "A Zip+4 code: prefix: %s, suffix: %s" code suffix
        | _ as n ->
            sprintf "Not a zip code: %s" n)

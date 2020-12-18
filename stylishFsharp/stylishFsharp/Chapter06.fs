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

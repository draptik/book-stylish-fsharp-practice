module Chapter06

open System

type MeterValue =
    | Standard of int
    | Economy of Day:int * Night:int // <- using tuple-label syntax

type MeterReading = {
    ReadingDate : DateTime
    MeterValue : MeterValue
}

let formatReading (meterReading: MeterReading) : string =
        
    let formatDate reading =
        // pattern matching record in let:
        let ({ ReadingDate = readingDate }) = reading
        readingDate.ToString("yyyy-MM-dd")
    
    let formattedMeterValue reading =
        // pattern matching record in let:
        let ({ MeterValue = meterValue }) = reading
        match meterValue with
        | Standard s -> sprintf " was %07i" s
        | Economy(Day=d; Night=n) -> sprintf ": Day: %07i Night: %07i" d n
    
    sprintf "Your reading on: %s%s"
        (formatDate meterReading)
        (formattedMeterValue meterReading) 

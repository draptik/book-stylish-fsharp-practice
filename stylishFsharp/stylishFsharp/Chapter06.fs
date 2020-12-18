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

    let leftPad num desiredStringLength paddingChar =
        let numString = num.ToString()
        let currentLength = numString |> String.length
        match currentLength <= desiredStringLength with
        | true ->
            String.replicate (desiredStringLength - currentLength) paddingChar + numString
            |> sprintf "%s"
        | false ->
            numString |> sprintf "%s"
        
    let formatNum n =
        leftPad n 7 "0" |> sprintf "%s"
    
    let formatDate reading =
        // pattern matching record in let:
        let ({ ReadingDate = readingDate }) = reading
        readingDate.ToString("yyyy-MM-dd")
    
    let formattedMeterValue reading =
        // pattern matching record in let:
        let ({ MeterValue = meterValue }) = reading
        match meterValue with
        | Standard s ->
            sprintf " was %s" (formatNum s)
        | Economy(Day=d; Night=n) ->
            sprintf ": Day: %s Night: %s" (formatNum d) (formatNum n)
    
    sprintf "Your reading on: %s%s"
        (formatDate meterReading)
        (formattedMeterValue meterReading) 

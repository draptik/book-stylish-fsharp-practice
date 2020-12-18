module Chapter09

open System

let add a b = a + b

let applyAndPrint f a b =
    let r = f a b
    printfn "%i" r
    r
    
let counter start =
    (* Neat trick to hide state inside a function *) 
    let mutable current = start
    
    fun () ->
        let this = current
        current <- current + 1
        this

(* This function returns a function that generates numbers
in a circular pattern between a specified range *)
let rangeCounter lowerBound upperBound =
    (* Neat trick to hide state inside a function *)
    let mutable current = lowerBound
    
    fun () ->
        let this = current
        let next = current + 1
        current <-
            if next > upperBound then
                lowerBound
            else
                next
        this

let featureScale targetMin targetMax actualMin actualMax x =
    targetMin + ((x - actualMin) * (targetMax - targetMin)) / (actualMax - actualMin)

let scale (data: seq<float>) =
    let minX = data |> Seq.min
    let maxX = data |> Seq.max
    
    data
    |> Seq.map (fun x -> featureScale 0. 1. minX maxX x)

let scale' (data: seq<float>) =
    let minX = data |> Seq.min
    let maxX = data |> Seq.max
    let zeroOneScaled =
        featureScale 0. 1. minX maxX
        
    data
    |> Seq.map zeroOneScaled

let applyAll (myFcnList : (int -> int) list) =
    (* This is advanced: Take some time to think about what is happening here *)
    myFcnList |> List.reduce (>>)
    
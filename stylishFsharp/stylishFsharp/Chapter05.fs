module Chapter05

open System

let clip ceil (xs : seq<_>) =
    xs
    |> Seq.map (fun x -> min x ceil)

let extremes (s : seq<float>) =
    let mutable min = Double.MaxValue
    let mutable max = Double.MinValue
    
    for item in s do
        if item < min then
            min <- item
        if item > max then
            max <- item
    min, max

let extremes' (s : seq<float>) =
    s |> Seq.min,
    s |> Seq.max

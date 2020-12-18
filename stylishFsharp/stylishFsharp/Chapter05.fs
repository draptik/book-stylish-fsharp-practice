module Chapter05

let clip ceil (xs : seq<_>) =
    xs
    |> Seq.map (fun x -> min x ceil)

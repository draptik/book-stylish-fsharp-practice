module Chapter09

let add a b = a + b

let applyAndPrint f a b =
    let r = f a b
    printfn "%i" r
    r
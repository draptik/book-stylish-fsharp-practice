module Chapter09

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
    
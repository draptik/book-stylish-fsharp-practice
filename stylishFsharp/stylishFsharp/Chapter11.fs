module Chapter11

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
    
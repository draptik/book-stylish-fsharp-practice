module Chapter13

module MinorPlanets =
    open System
    
    let charArray (s : string) =
        s.ToCharArray()
        
    let toDouble (s : string) =
        match Double.TryParse(s) with
        | true, x -> Some x
        | false, _ -> None
    
    let toChar (s : string) =
        if String.IsNullOrWhiteSpace(s) then None
        else Some (s.[0])

    let toInt (s : string) =
        match Int32.TryParse(s) with
        | true, x -> Some x
        | false, _ -> None
    
    let columnAsString startInd endInd (line : string) =
        line.Substring(startInd-1, endInd-startInd).Trim()

    let columnAsCharArray startInd endInd (line : string) =
        charArray(columnAsString startInd endInd line)

    let columnAsInt startInd endInd (line : string) =
        toInt(columnAsString startInd endInd line)

    let columnAsDouble startInd endInd (line : string) =
        toDouble(columnAsString startInd endInd line)

    let columnAsChar startInd endInd (line : string) =
        toChar(columnAsString startInd endInd line)

    type ObservationRange =
        | SingleOpposition of int
        | MultiOpposition of int * int
        
    let rangeFromLine (oppositions : int option) (line : string) =
        match oppositions with
        | None -> None
        | Some o when o = 1 ->
            line  |> columnAsInt 128 131
            |> Option.map SingleOpposition
        | Some o ->
            match (line |> columnAsInt 128 131), (line |> columnAsInt 128 136) with
            | Some (firstObservedYear), Some(lastObservedYear) ->
                MultiOpposition(firstObservedYear, lastObservedYear) |> Some
            | _ -> None
            
    type MinorPlanet = {
        Designation : string; AbsMag : float option
        SlopeParam:float option; Epoch:string
        MeanAnom:float option; Perihelion:float option
        Node:float option; Inclination:float option
        OrbEcc:float option; MeanDaily:float option
        SemiMajor:float option; Uncertainty:char option
        Reference: string; Observations:int option
        Oppositions:int option; Range:ObservationRange option
        RmsResiduals:double option; PerturbersCoarse:string
        PerturbersPrecise:string;ComputerName:string
        Flags:char[];ReadableDesignation:string
        LastOpposition: string
    }
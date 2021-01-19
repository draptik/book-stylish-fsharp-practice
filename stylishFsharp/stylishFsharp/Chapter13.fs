module Chapter13

module Convert =
    open System

    let charArray (s : string) =
        s.ToCharArray()
        
    let tryDouble (s : string) =
        match Double.TryParse(s) with
        | true, x -> Some x
        | false, _ -> None
    
    let tryChar (s : string) =
        if String.IsNullOrWhiteSpace(s) then None
        else Some (s.[0])

    let tryInt (s : string) =
        match Int32.TryParse(s) with
        | true, x -> Some x
        | false, _ -> None

module Column =
    open Convert
    
    let asString startInd endInd (line : string) =
        let len =  endInd - startInd + 1
        line
            .Substring(startInd-1, len)
            .Trim()

    let asCharArray startInd endInd (line : string) =
        charArray(asString startInd endInd line)

    let tryAsInt startInd endInd (line : string) =
        tryInt(asString startInd endInd line)

    let tryAsDouble startInd endInd (line : string) =
        tryDouble(asString startInd endInd line)

    let tryAsChar startInd endInd (line : string) =
        tryChar(asString startInd endInd line)
    

module MinorPlanets =
    open Convert
    open Column
    
    type ObservationRange =
        | SingleOpposition of int
        | MultiOpposition of int * int
        
    let rangeFromLine (oppositions : int option) (line : string) =
        match oppositions with
        | None -> None
        | Some o when o = 1 ->
            line  |> tryAsInt 128 131
            |> Option.map SingleOpposition
        | Some o ->
            match (line |> tryAsInt 128 131), (line |> tryAsInt 128 136) with
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
        RmsResidual:double option; PerturbersCoarse:string
        PerturbersPrecise:string;ComputerName:string
        Flags:char[];ReadableDesignation:string
        LastOpposition: string
    }
    
    let private create (line : string) =
        let oppositions = line |> asString 124 126 |> tryInt
        let range = line |> rangeFromLine oppositions
        
        {
            Designation = asString 1 7 line
            AbsMag = tryAsDouble 9 13 line
            SlopeParam = tryAsDouble 15 19 line
            Epoch = asString 21 25 line
            MeanAnom = tryAsDouble 27 35 line
            Perihelion = tryAsDouble 38 46 line
            Node = tryAsDouble 49 57 line
            Inclination = tryAsDouble 60 68 line
            OrbEcc = tryAsDouble 71 79 line
            MeanDaily = tryAsDouble 81 91 line
            SemiMajor = tryAsDouble 93 103 line
            Uncertainty = tryAsChar 106 106 line
            Reference = asString 108 116 line
            Observations = tryAsInt 118 122 line
            Oppositions = oppositions
            Range = range
            RmsResidual = tryAsDouble 138 141 line
            PerturbersCoarse = asString 143 145 line
            PerturbersPrecise = asString 147 149 line
            ComputerName = asString 151 160 line
            Flags = asCharArray 162 165 line
            ReadableDesignation = asString 167 194 line
            LastOpposition = asString 195 202 line
        }
    
    let createFromData (data : seq<string>) =
        data
        |> Seq.skipWhile (fun line -> line.StartsWith("----------") |> not)
        |> Seq.skip 1
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.map (fun line -> create line)
        
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
        RmsResidual:double option; PerturbersCoarse:string
        PerturbersPrecise:string;ComputerName:string
        Flags:char[];ReadableDesignation:string
        LastOpposition: string
    }
    
    let private create (line : string) =
        let oppositions = line |> columnAsString 124 126 |> toInt
        let range = line |> rangeFromLine oppositions
        
        {
            Designation = columnAsString 1 7 line
            AbsMag = columnAsDouble 9 13 line
            SlopeParam = columnAsDouble 15 19 line
            Epoch = columnAsString 21 25 line
            MeanAnom = columnAsDouble 27 35 line
            Perihelion = columnAsDouble 38 46 line
            Node = columnAsDouble 49 57 line
            Inclination = columnAsDouble 60 68 line
            OrbEcc = columnAsDouble 71 79 line
            MeanDaily = columnAsDouble 81 91 line
            SemiMajor = columnAsDouble 93 103 line
            Uncertainty = columnAsChar 106 106 line
            Reference = columnAsString 108 116 line
            Observations = columnAsInt 118 122 line
            Oppositions = oppositions
            Range = range
            RmsResidual = columnAsDouble 138 141 line
            PerturbersCoarse = columnAsString 143 145 line
            PerturbersPrecise = columnAsString 147 149 line
            ComputerName = columnAsString 151 160 line
            Flags = columnAsCharArray 162 165 line
            ReadableDesignation = columnAsString 167 194 line
            LastOpposition = columnAsString 195 202 line
        }
    
    let createFromData (data : seq<string>) =
        data
        |> Seq.skipWhile (fun line -> line.StartsWith("----------") |> not)
        |> Seq.skip 1
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.map (fun line -> create line)
        
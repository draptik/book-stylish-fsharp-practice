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
    let asString startInd endInd (line : string) =
        let len =  endInd - startInd + 1
        line
            .Substring(startInd-1, len)
            .Trim()

    let asCharArray startInd endInd =
        (asString startInd endInd) >> Convert.charArray

    let tryAsInt startInd endInd =
        (asString startInd endInd) >> Convert.tryInt

    let tryAsDouble startInd endInd =
        (asString startInd endInd) >> Convert.tryDouble

    let tryAsChar startInd endInd =
        (asString startInd endInd) >> Convert.tryChar

module ObservationRange =
    type Range =
        private
        | SingleOpposition of ArcLengthDays:int
        | MultiOpposition of FirstYear:int * LastYear:int

    let fromLine (oppositions : int option) (line : string) =
        match oppositions with
        | None ->
            None
        | Some o when o = 1 ->
            line
            |> Column.tryAsInt 128 131
            |> Option.map SingleOpposition
        | Some _ ->
            let firstYear = line |> Column.tryAsInt 128 131
            let lastYear = line |> Column.tryAsInt 128 136
            match firstYear, lastYear with
            | Some (fy), Some(ly) ->
                MultiOpposition(FirstYear=fy, LastYear=ly) |> Some
            | _ ->
                None

module MinorPlanets =
            
    type MinorPlanet = {
        Designation : string; AbsMag : float option
        SlopeParam:float option; Epoch:string
        MeanAnom:float option; Perihelion:float option
        Node:float option; Inclination:float option
        OrbEcc:float option; MeanDaily:float option
        SemiMajor:float option; Uncertainty:char option
        Reference: string; Observations:int option
        Oppositions:int option; Range:ObservationRange.Range option
        RmsResidual:double option; PerturbersCoarse:string
        PerturbersPrecise:string;ComputerName:string
        Flags:char[];ReadableDesignation:string
        LastOpposition: string
    }
    
    let private create (line : string) =
        let oppositions = line |> Column.asString 124 126 |> Convert.tryInt
        let range = line |> ObservationRange.fromLine oppositions
        
        {
            Designation = Column.asString 1 7 line
            AbsMag = Column.tryAsDouble 9 13 line
            SlopeParam = Column.tryAsDouble 15 19 line
            Epoch = Column.asString 21 25 line
            MeanAnom = Column.tryAsDouble 27 35 line
            Perihelion = Column.tryAsDouble 38 46 line
            Node = Column.tryAsDouble 49 57 line
            Inclination = Column.tryAsDouble 60 68 line
            OrbEcc = Column.tryAsDouble 71 79 line
            MeanDaily = Column.tryAsDouble 81 91 line
            SemiMajor = Column.tryAsDouble 93 103 line
            Uncertainty = Column.tryAsChar 106 106 line
            Reference = Column.asString 108 116 line
            Observations = Column.tryAsInt 118 122 line
            Oppositions = oppositions
            Range = range
            RmsResidual = Column.tryAsDouble 138 141 line
            PerturbersCoarse = Column.asString 143 145 line
            PerturbersPrecise = Column.asString 147 149 line
            ComputerName = Column.asString 151 160 line
            Flags = Column.asCharArray 162 165 line
            ReadableDesignation = Column.asString 167 194 line
            LastOpposition = Column.asString 195 202 line
        }
    
    let createFromData (data : seq<string>) =
        data
        |> Seq.skipWhile (fun line -> line.StartsWith("----------") |> not)
        |> Seq.skip 1
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.map (fun line -> create line)
        
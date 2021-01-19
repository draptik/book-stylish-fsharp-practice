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

module MinorPlanet =

    type Body = {
        /// Number of provisional designation (packed format)
        Designation : string
        /// Absolute magnitude
        H : float option
        /// Slope parameter
        G : float option
        /// Epoch in packed form
        Epoch : string
        /// Mean anomaly of the epoch (degrees)
        M : float option
        /// Argument of periphelion, J2000.0 (degrees)
        Perihelion : float option
        /// Longitude of the ascending node, J2000.0 (degrees)
        Node : float option
        /// Inclination to the ecliptic, J2000.0 (degrees) 
        Inclination : float option
        /// Orbital eccentricity
        e : float option
        /// Mean daily motion (degrees per day)
        n : float option
        /// Semimajor axis (AU)
        a : float option
        /// Uncertainty parameter
        Uncertainty : char option
        /// Reference
        Reference : string
        /// Number of observations
        Observations : int option
        /// Number of oppositions
        Oppositions : int option
        /// Year of first and last observation,
        /// or arc length in days
        Range : ObservationRange.Range option
        /// RMS residual (arcseconds)
        RmsResidual : double option
        /// Coarse indicator of perturbers
        PerturbersCoarse : string
        /// Precise indicator of perturbers
        PerturbersPrecise : string
        /// Computer name
        ComputerName : string
        /// Flags
        Flags : char[]
        /// Readable designation
        ReadableDesignation : string
        /// Date of last observation included in orbit solution (YYYYMMDD)
        LastOpposition : string }    

    let fromMpcOrbLine (line : string) =
        let oppositions = line |> Column.asString 124 126 |> Convert.tryInt
        let range = line |> ObservationRange.fromLine oppositions
        
        {
            Designation =         line |> Column.asString    1     7   
            H =                   line |> Column.tryAsDouble 9    13  
            G =                   line |> Column.tryAsDouble 15   19  
            Epoch =               line |> Column.asString    21   25  
            M =                   line |> Column.tryAsDouble 27   35  
            Perihelion =          line |> Column.tryAsDouble 38   46  
            Node =                line |> Column.tryAsDouble 49   57  
            Inclination =         line |> Column.tryAsDouble 60   68  
            e =                   line |> Column.tryAsDouble 71   79  
            n =                   line |> Column.tryAsDouble 81   91  
            a =                   line |> Column.tryAsDouble 93  103 
            Uncertainty =         line |> Column.tryAsChar   106 106 
            Reference =           line |> Column.asString    108 116 
            Observations =        line |> Column.tryAsInt    118 122 
            Oppositions =         oppositions
            Range =               range
            RmsResidual =         line |> Column.tryAsDouble 138 141 
            PerturbersCoarse =    line |> Column.asString    143 145 
            PerturbersPrecise =   line |> Column.asString    147 149 
            ComputerName =        line |> Column.asString    151 160 
            Flags =               line |> Column.asCharArray 162 165 
            ReadableDesignation = line |> Column.asString    167 194 
            LastOpposition =      line |> Column.asString    195 202 
        }    

    let private skipHeader (data : seq<string>) =
        data
        |> Seq.skipWhile (fun line ->
            line.StartsWith("----------") |> not)
        |> Seq.skip 1
        
    let fromMpcOrbData (data : seq<string>) =
        data
        |> skipHeader
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.map fromMpcOrbLine

module Exercise13_1 =

    module Helper =
        open System.IO
        
        let createReadOnlyFile name =
            File.WriteAllText (name, "test")
            let fi = FileInfo(name)
            fi.IsReadOnly <- true
        
        let deleteFile name =
            let fi = FileInfo(name)
            fi.IsReadOnly <- false
            File.Delete (name)
    
    module InitialCode =
        open System.IO
        open System.Text.RegularExpressions
        
        let find pattern dir =
            let re = Regex(pattern)
            Directory.EnumerateFiles
                (dir, "*.*", SearchOption.AllDirectories)
            |> Seq.filter (fun path -> re.IsMatch(Path.GetFileName(path)))
            |> Seq.map (fun path ->
                FileInfo(path))
            |> Seq.filter (fun fi ->
                fi.Attributes.HasFlag(FileAttributes.ReadOnly))
            |> Seq.map (fun fi -> fi.Name)
                
    module ImprovedCode =
        open System.IO
        open System.Text.RegularExpressions
        
        module FileSearch =
            module private FileName =
                let isMatch pattern =
                    let re = Regex(pattern)
                    fun (path : string) ->
                        let fileName = Path.GetFileName(path)
                        re.IsMatch(fileName)
            
            module private FileAttributes =
                let hasFlag flag filePath =
                    FileInfo(filePath)
                        .Attributes
                        .HasFlag(flag)
                        
        
            /// Search below path for files whose file names match the specified
            /// regular expression, and which have the 'read-only' attribute set.
            let findReadOnly pattern dir =
                Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                |> Seq.filter (FileName.isMatch pattern)
                |> Seq.filter (FileAttributes.hasFlag FileAttributes.ReadOnly)
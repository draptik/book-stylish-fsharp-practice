module stylishFsharp.Console.Chapter12

open System
open System.Text
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Chapter12

module InappropriateCollectionType =
    module Old =
        
        /// Listing 12-5
        let sample interval data =
            [
                let max = (List.length data) - 1
                for i in 0..interval..max ->
                    data.[i]
            ]

    module New =
        /// Listing 12-5
        //        let sample interval data =
        //            [
        //                let max = (List.length data) - 1
        //                for i in 0..interval..max ->
        //                    data.[i]
        //            ]

        /// Listing 12-8
        //        let sample interval data =
        //            data
        //            |> List.indexed
        //            |> List.filter (fun (i, _) ->
        //                i % interval = 0)
        //            |> List.map snd
        //        (*
        //            | Method |       Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |     Gen 2 |   Allocated |
        //            |------- |-----------:|---------:|---------:|----------:|----------:|----------:|------------:|
        //            |    Old | 1,325.1 ms | 24.79 ms | 21.97 ms |         - |         - |         - |    33.91 KB |
        //            |    New |   135.1 ms |  2.67 ms |  6.75 ms | 8750.0000 | 5000.0000 | 1250.0000 | 62562.99 KB |
        //        
        //            - Pros: 10x faster
        //            - Cons: a lot of garbage collection going on (we have 3 lists now!)
        //        *)

        /// Listing 12-10                
        //        let sample interval data =
        //            data
        //            |> Array.indexed
        //            |> Array.filter (fun (i, _) ->
        //                i % interval = 0)
        //            |> Array.map snd
        //        (*
        //            | Method |        Mean |     Error |    StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
        //            |------- |------------:|----------:|----------:|----------:|----------:|---------:|------------:|
        //            |    Old | 1,320.65 ms | 13.895 ms | 12.997 ms |         - |         - |        - |    33.91 KB |
        //            |    New |    73.67 ms |  1.065 ms |  0.996 ms | 4428.5714 | 2571.4286 | 714.2857 | 39201.71 KB |
        //            
        //            - Using Array instead of List
        //            - same Pros and Cons as before
        //        *)

        /// Listing 12-13                
        //        let sample interval data =
        //            data
        //            |> Seq.indexed
        //            |> Seq.filter (fun (i, _) ->
        //                i % interval = 0)
        //            |> Seq.map snd
        //        (*
        //            | Method |        Mean |     Error |    StdDev |     Gen 0 |   Gen 1 | Gen 2 |   Allocated |
        //            |------- |------------:|----------:|----------:|----------:|--------:|------:|------------:|
        //            |    Old | 1,311.84 ms | 13.723 ms | 12.165 ms |         - |       - |     - |    35.68 KB |
        //            |    New |    24.09 ms |  0.470 ms |  0.393 ms | 3812.5000 | 31.2500 |     - | 31274.59 KB |
        //            
        //            - Even faster
        //            - Pros: less garbage collection (the book only has Gen 0, don't know why there is GC-Gen1 here)
        //        *)

        /// Listing 12-16                
        //        let sample interval data =
        //            [|
        //                let max = (Array.length data) - 1
        //                for i in 0..interval..max ->
        //                    data.[i]
        //            |]
        //        (*
        //            | Method |            Mean |         Error |        StdDev |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
        //            |------- |----------------:|--------------:|--------------:|-------:|-------:|------:|----------:|
        //            |    Old | 1,327,013.16 us | 17,049.323 us | 15,947.947 us |      - |      - |     - |  33.91 KB |
        //            |    New |        16.81 us |      0.229 us |      0.214 us | 3.9063 | 0.1221 |     - |   32.1 KB |
        //            
        //            - Even faster. NOTE: measurement time is now in microseconds (us=microseconds !), not milliseconds (ms) as before
        //            - Pros: even less garbage collection        
        //        *)
            
        /// Listing 12-18                
        let sample interval data =
            [|
                let max =
                    ( (data |> Array.length |> float) / (float interval)
                      |> ceil
                      |> int ) - 1
                    
                for i in 0..max ->
                    data.[i * interval]
            |]
        (*
            | Method |            Mean |         Error |        StdDev |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
            |------- |----------------:|--------------:|--------------:|-------:|-------:|------:|----------:|
            |    Old | 1,338,237.43 us | 16,953.601 us | 15,858.409 us |      - |      - |     - |  33.91 KB |
            |    New |        15.23 us |      0.125 us |      0.111 us | 3.9215 | 0.1373 |     - |  32.09 KB |
            
            - Similar to previous result from listing 12-16
            - Cons:
                - lowers "motivational transparency" of the code, by making it a little less obvious what the author was intending to do
                - micro-optimization (architecture / compiler)
                - more risk, because code has complicated calculation with potential of-by-one error        
        *)

module ShortTermObjects =
    
    type Point3d(x : float, y : float, z : float) =
        member __.X = x
        member __.Y = y
        member __.Z = z
        member val Description = "" with get, set
        member this.DistanceFrom(that : Point3d) =
            (that.X - this.X) ** 2. +
            (that.Y - this.Y) ** 2. +
            (that.Z - this.Y) ** 2.
            |> sqrt
        override this.ToString() =
            sprintf "X: %f, Y: %f, Z: %f" this.X this.Y this.Z

    type Float3 = (float * float * float)
    
    module Old =
        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
            let here = Point3d(here)
            coords
            |> Array.map Point3d
            |> Array.filter (fun there ->
                there.DistanceFrom(here) <= radius)
            |> Array.map (fun p3d -> p3d.X, p3d.Y, p3d.Z)
    
    module New =
        
        /// Listing 12.22
        //        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        //            let here = Point3d(here)
        //            coords
        //            |> Array.map Point3d
        //            |> Array.filter (fun there ->
        //                there.DistanceFrom(here) <= radius)
        //            |> Array.map (fun p3d -> p3d.X, p3d.Y, p3d.Z)
        //        (*
        //            | Method |     Mean |   Error |  StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        //            |------- |---------:|--------:|--------:|----------:|----------:|---------:|----------:|
        //            |    Old | 146.1 ms | 1.74 ms | 1.62 ms | 6500.0000 | 3500.0000 | 750.0000 |  53.83 MB |
        //            |    New | 145.4 ms | 2.17 ms | 2.03 ms | 6500.0000 | 3500.0000 | 750.0000 |  53.83 MB |
        //
        //            - Baseline (New == Old)
        //        *)
        
        /// Listing 12.25
        //        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        //            let here = Point3d(here)
        //            coords
        //            |> Seq.map Point3d
        //            |> Seq.filter (fun there ->
        //                there.DistanceFrom(here) <= radius)
        //            |> Seq.map (fun p3d -> p3d.X, p3d.Y, p3d.Z)
        //            |> Seq.toArray
        //        (*
        //            | Method |      Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        //            |------- |----------:|---------:|---------:|----------:|----------:|---------:|----------:|
        //            |    Old | 145.71 ms | 2.456 ms | 2.297 ms | 6500.0000 | 3500.0000 | 750.0000 |  53.83 MB |
        //            |    New |  88.19 ms | 0.432 ms | 0.337 ms | 5666.6667 |  333.3333 |        - |  46.16 MB |
        //
        //            - Using Seq instead of Array: Only slightly faster
        //        *)
        
        /// Listing 12.27
        //        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        //            let distance (p1 : float*float*float) (p2 : float*float*float) =
        //                let x1, y1, z1 = p1
        //                let x2, y2, z2 = p2
        //                (x1 - x2) ** 2. +
        //                (y1 - y2) ** 2. +
        //                (z1 - z2) ** 2.
        //                |> sqrt
        //            coords
        //            |> Array.filter (fun there ->
        //                distance here there <= radius)
        //        (*
        //            | Method |      Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
        //            |------- |----------:|---------:|---------:|----------:|----------:|---------:|------------:|
        //            |    Old | 145.47 ms | 2.520 ms | 2.357 ms | 6500.0000 | 3500.0000 | 750.0000 | 55122.01 KB |
        //            |    New |  57.91 ms | 0.463 ms | 0.410 ms |         - |         - |        - |   126.41 KB |
        //
        //            - Pros:
        //                - faster
        //                - no garbage collection
        //        *)
        
        /// Listing 12.29
        //        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        //            let distance x1 y1 z1 x2 y2 z2 =
        //                (x1 - x2) ** 2. +
        //                (y1 - y2) ** 2. +
        //                (z1 - z2) ** 2.
        //                |> sqrt
        //                
        //            let x1, y1, z1 = here
        //
        //            coords
        //            |> Array.filter (fun (x2, y2, z2) ->
        //                distance x1 y1 z1 x2 y2 z2 <= radius)
        //        (*
        //            | Method |      Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
        //            |------- |----------:|---------:|---------:|----------:|----------:|---------:|------------:|
        //            |    Old | 143.54 ms | 1.442 ms | 1.204 ms | 6500.0000 | 3500.0000 | 750.0000 | 55122.24 KB |
        //            |    New |  63.67 ms | 1.236 ms | 1.773 ms |         - |         - |        - |   126.35 KB |
        //
        //            - no benefit compared to previous solution (12.27)
        //        *)
        
        /// Listing 12.31
        //        let withinRadius (radius : float) (here : struct(float*float*float)) (coords : struct(float*float*float)[]) =
        //            let distance p1 p2 =
        //                let struct(x1, y1, z1) = p1
        //                let struct(x2, y2, z2) = p2
        //                (x1 - x2) ** 2. +
        //                (y1 - y2) ** 2. +
        //                (z1 - z2) ** 2.
        //                |> sqrt
        //                
        //            coords
        //            |> Array.filter (fun there ->
        //                distance here there <= radius)
        //        (*
        //            | Method |      Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
        //            |------- |----------:|---------:|---------:|----------:|----------:|---------:|------------:|
        //            |    Old | 146.77 ms | 2.282 ms | 2.134 ms | 6500.0000 | 3500.0000 | 750.0000 | 55122.25 KB |
        //            |    New |  59.95 ms | 0.369 ms | 0.327 ms |         - |         - |        - |   134.73 KB |
        //
        //            - no benefit compared to previous solution (12.27 or 12.29)
        //        *)
        
        /// Listing 12.34
        //        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        //            let distance x1 y1 z1 x2 y2 z2 =
        //                pown (x1 - x2) 2 +
        //                pown (y1 - y2) 2 +
        //                pown (z1 - z2) 2
        //                |> sqrt
        //            let x1, y1, z1 = here
        //            coords
        //            |> Array.filter (fun (x2, y2, z2) ->
        //                distance x1 y1 z1 x2 y2 z2 <= radius)
        //        (*
        //            | Method |      Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
        //            |------- |----------:|---------:|---------:|----------:|----------:|---------:|------------:|
        //            |    Old | 145.16 ms | 2.490 ms | 2.329 ms | 6500.0000 | 3500.0000 | 750.0000 | 55122.01 KB |
        //            |    New |  11.26 ms | 0.073 ms | 0.069 ms |         - |         - |        - |   126.32 KB |
        //
        //            - Pros: faster by factor 10
        //        *)
        
        /// Listing 12.37
        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
            let distance x1 y1 z1 x2 y2 z2 =
                let dx = x1 - x2
                let dy = y1 - y2
                let dz = z1 - z2
                dx * dx +
                dy * dy +
                dz * dz
                |> sqrt
            let x1, y1, z1 = here
            coords
            |> Array.filter (fun (x2, y2, z2) ->
                distance x1 y1 z1 x2 y2 z2 <= radius)
        (*
            | Method |       Mean |     Error |    StdDev |     Gen 0 |     Gen 1 |    Gen 2 |   Allocated |
            |------- |-----------:|----------:|----------:|----------:|----------:|---------:|------------:|
            |    Old | 146.641 ms | 2.9144 ms | 2.9929 ms | 6500.0000 | 3500.0000 | 750.0000 | 55121.36 KB |
            |    New |   4.874 ms | 0.0194 ms | 0.0181 ms |         - |         - |        - |   126.32 KB |

            - fastest ;-)
        *)

module NaiveStringBuilder =
    open System
    module Old =
        
        /// Listing 12-39
        let private buildLine (data : float[]) =
            let mutable result = ""
            for x in data do
                result <- sprintf "%s%f," result x
            result.TrimEnd(',')
        
        let buildCsv (data : float[,]) =
            let mutable result = ""
            for r in 0..(data |> Array2D.length1) - 1 do
                let row = data.[r, *]
                let rowString = row |> buildLine
                result <- sprintf "%s%s%s" result rowString Environment.NewLine
            result

    module New =
        
        /// Listing 12-39
        //        let private buildLine (data : float[]) =
        //            let mutable result = ""
        //            for x in data do
        //                result <- sprintf "%s%f," result x
        //            result.TrimEnd(',')
        //        
        //        let buildCsv (data : float[,]) =
        //            let mutable result = ""
        //            for r in 0..(data |> Array2D.length1) - 1 do
        //                let row = data.[r, *]
        //                let rowString = row |> buildLine
        //                result <- sprintf "%s%s%s" result rowString Environment.NewLine
        //            result
        //        (*
        //            | Method |     Mean |    Error |   StdDev |       Gen 0 |       Gen 1 |       Gen 2 | Allocated |
        //            |------- |---------:|---------:|---------:|------------:|------------:|------------:|----------:|
        //            |    Old | 808.2 ms | 13.03 ms | 16.00 ms | 336000.0000 | 145000.0000 | 136000.0000 |   3.05 GB |
        //            |    New | 811.0 ms | 15.63 ms | 14.62 ms | 339000.0000 | 148000.0000 | 139000.0000 |   3.05 GB |
        //
        //            - baseline
        //            - Cons: 3GB of allocated memory (!) and a lot of garbage collection
        //        *)
        
        /// Listing 12-42
        //        let private buildLine (data : float[]) =
        //            let sb = StringBuilder()
        //            for x in data do
        //                sb.Append(sprintf "%f," x) |> ignore
        //            sb.ToString().TrimEnd(',')
        //        
        //        let buildCsv (data : float[,]) =
        //            let sb = StringBuilder()
        //            for r in 0..(data |> Array2D.length1) - 1 do
        //                let row = data.[r, *]
        //                let rowString = row |> buildLine
        //                sb.AppendLine(rowString) |> ignore
        //            sb.ToString()
        //        (*
        //            | Method |      Mean |     Error |    StdDev |       Gen 0 |       Gen 1 |       Gen 2 |  Allocated |
        //            |------- |----------:|----------:|----------:|------------:|------------:|------------:|-----------:|
        //            |    Old | 816.67 ms | 15.677 ms | 25.315 ms | 335000.0000 | 144000.0000 | 135000.0000 | 3127.22 MB |
        //            |    New |  85.10 ms |  0.484 ms |  0.404 ms |   9833.3333 |   1833.3333 |    833.3333 |   81.17 MB |
        //
        //            - Pros:
        //                - 10x faster
        //                - less allocated memory
        //                - less garbage collection
        //        *)

        /// Listing 12-44
        //        let private buildLine (data : float[]) =
        //            let cols = data |> Array.map (sprintf "%f")
        //            String.Join(',', cols)
        //        
        //        let buildCsv (data : float[,]) =
        //            let sb = StringBuilder()
        //            for r in 0..(data |> Array2D.length1) - 1 do
        //                let row = data.[r, *]
        //                let rowString = row |> buildLine
        //                sb.AppendLine(rowString) |> ignore
        //            sb.ToString()
        //        (*
        //            | Method |      Mean |     Error |    StdDev |       Gen 0 |       Gen 1 |       Gen 2 |  Allocated |
        //            |------- |----------:|----------:|----------:|------------:|------------:|------------:|-----------:|
        //            |    Old | 819.02 ms | 15.852 ms | 23.236 ms | 335000.0000 | 144000.0000 | 135000.0000 | 3127.24 MB |
        //            |    New |  73.19 ms |  0.674 ms |  0.597 ms |   5714.2857 |   2142.8571 |    857.1429 |   47.32 MB |
        //        
        //            - Pros:
        //                - same as 12-42
        //                - easier to read & optimized
        //        *)

        /// Listing 12-46
        let private buildLine (data : float[]) =
            let cols = data |> Array.Parallel.map (sprintf "%f")
            String.Join(',', cols)
        
        let buildCsv (data : float[,]) =
            let sb = StringBuilder()
            for r in 0..(data |> Array2D.length1) - 1 do
                let row = data.[r, *]
                let rowString = row |> buildLine
                sb.AppendLine(rowString) |> ignore
            sb.ToString()
        (*
            | Method |      Mean |     Error |    StdDev |       Gen 0 |       Gen 1 |       Gen 2 |  Allocated |
            |------- |----------:|----------:|----------:|------------:|------------:|------------:|-----------:|
            |    Old | 807.54 ms | 15.713 ms | 23.518 ms | 336000.0000 | 145000.0000 | 136000.0000 | 3127.22 MB |
            |    New |  35.05 ms |  0.657 ms |  1.832 ms |   6250.0000 |   2093.7500 |    968.7500 |   49.53 MB |
        
            - Pros: 2x faster than Listing 12-44
        *)
    
module Harness =
    
    [<MemoryDiagnoser>]
    type HarnessInappropriateCollectionType() =

        let r = Random()
        let list = List.init 1_000_000 (fun _ -> r.NextDouble())
        let array = list |> Array.ofList
        
        [<Benchmark>]
        member __.Old() =
            list
            |> InappropriateCollectionType.Old.sample 1000
            |> ignore
        
        [<Benchmark>]
        member __.New() =
            array
            |> InappropriateCollectionType.New.sample 1000
            |> Array.ofSeq
            |> ignore

    [<MemoryDiagnoser>]
    type HarnessShortTermObjects() =

        let r = Random(1)
        let coords =
            Array.init 1_000_000 (fun _ ->
                r.NextDouble(), r.NextDouble(), r.NextDouble())
        let here = (0., 0., 0.)
        
        [<Benchmark>]
        member __.Old() =
            coords
            |> ShortTermObjects.Old.withinRadius 0.1 here
            |> ignore
        
        [<Benchmark>]
        member __.New() =
            coords
            |> ShortTermObjects.New.withinRadius 0.1 here
            |> ignore

    [<MemoryDiagnoser>]
    type HarnessNaiveStringBuilder() =
    
        let data =
            Array2D.init 500 500 (fun x y ->
                x * y |> float)
        
        [<Benchmark>]
        member __.Old() =
            data
            |> NaiveStringBuilder.Old.buildCsv
            |> ignore
            
        [<Benchmark>]
        member __.New() =
            data
            |> NaiveStringBuilder.New.buildCsv
            |> ignore

    open Chapter12.Exercise12_1
    open Chapter12.Exercise12_2
    
    [<MemoryDiagnoser>]
    type HarnessExercise12_01() =
        let a = [{Id = 1};{Id = 2};{Id = 3};{Id = 4};{Id = 5};{Id = 6};]
        let b = [{Id = 7};{Id = 8};{Id = 9};{Id = 10};{Id = 11};{Id = 12};]
        let a' = a |> Array.ofList
        let b' = b |> Array.ofList

        [<Benchmark>]
        member __.Old() =
            addTransactionsBaseline a b |> ignore
            
        [<Benchmark>]
        member __.New() =
            addTransactions a' b' |> ignore
            
    [<MemoryDiagnoser>]
    type HarnessExercise12_02() =
        let r = Random(1)
        let coords =
            Array.init 1_000_000 (fun _ ->
                r.NextDouble(), r.NextDouble(), r.NextDouble())
        let here = (0., 0., 0.)
        
        [<Benchmark>]
        member __.Old() =
            coords
            |> withinRadiusBaseline 0.1 here
            |> ignore
            
        [<Benchmark>]
        member __.New() =
            coords
            |> withinRadius 0.1 here
            |> ignore
        (*
            | Method |     Mean |     Error |    StdDev |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
            |------- |---------:|----------:|----------:|---------:|---------:|---------:|-----------:|
            |    Old | 4.739 ms | 0.0244 ms | 0.0216 ms |        - |        - |        - |  137.59 KB |
            |    New | 4.998 ms | 0.0490 ms | 0.0434 ms | 140.6250 | 140.6250 | 140.6250 | 7969.78 KB |

            - New vs Old: no performance gain (actually even slightly slower)
            - Even worse: We are creating a new array (!) -> garbage collection
        *)

    open Chapter12.Exercise12_3
                
    [<MemoryDiagnoser>]
    type HarnessExercise12_03() =
    
        let data =
            Array2D.init 500 500 (fun x y ->
                x * y |> float)
        
        [<Benchmark>]
        member __.Old() =
            data
            |> buildCsvBaseline |> ignore

        [<Benchmark>]
        member __.New() =
            data
            |> buildCsvBaseline |> ignore
        (*
        *)
            
let runChapter12_Case1_InappropriateCollectionType () =
    BenchmarkRunner.Run<Harness.HarnessInappropriateCollectionType>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Case2_ShortTermObjects () =
    BenchmarkRunner.Run<Harness.HarnessShortTermObjects>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Case3_NaiveStringBuilder () =
    BenchmarkRunner.Run<Harness.HarnessNaiveStringBuilder>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Exercise_12_01 () =
    BenchmarkRunner.Run<Harness.HarnessExercise12_01>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Exercise_12_02 () =
    BenchmarkRunner.Run<Harness.HarnessExercise12_02>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Exercise_12_03 () =
    BenchmarkRunner.Run<Harness.HarnessExercise12_03>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
    
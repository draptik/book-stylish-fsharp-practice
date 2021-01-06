module stylishFsharp.Console.Chapter12

open System
open System.Drawing
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

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
        let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
            let here = Point3d(here)
            coords
            |> Array.map Point3d
            |> Array.filter (fun there ->
                there.DistanceFrom(here) <= radius)
            |> Array.map (fun p3d -> p3d.X, p3d.Y, p3d.Z)
        (*
            | Method |     Mean |   Error |  StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
            |------- |---------:|--------:|--------:|----------:|----------:|---------:|----------:|
            |    Old | 146.1 ms | 1.74 ms | 1.62 ms | 6500.0000 | 3500.0000 | 750.0000 |  53.83 MB |
            |    New | 145.4 ms | 2.17 ms | 2.03 ms | 6500.0000 | 3500.0000 | 750.0000 |  53.83 MB |

            - Baseline (New == Old)
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

let runChapter12_Case1_InappropriateCollectionType () =
    BenchmarkRunner.Run<Harness.HarnessInappropriateCollectionType>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
let runChapter12_Case2_ShortTermObjects () =
    BenchmarkRunner.Run<Harness.HarnessShortTermObjects>()
    |> printfn "%A"
    
    Console.ReadKey() |> ignore
    
    
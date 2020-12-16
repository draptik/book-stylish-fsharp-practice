module Chapter04Tests

open System
open Xunit
open FsUnit.Xunit
open Swensen.Unquote

module Exercise04_01_Tests =

    open Chapter04.Houses
    open Chapter04.Exercise04_01
    
    [<Fact>]
    let ``Exercise 4-1: Transforming data items to list of strings`` () =
        let actual = housePrices
        actual |> should haveLength 20
        actual.[0] |> should startWith "Address: "
    
    [<Fact>]
    let ``Exercise 4-1: Transforming data items to list of strings using unquote`` () =
        let actual = housePrices
        test <@ actual.Length = 20 @>

(* This chapter uses System.Random -> white box testing *)
module Exercise04_Whitebox_Tests =

    open Chapter04.Houses
        
    [<Fact>]
    let ``Exercise 4-2: Calculate average prices`` () =
        let actual = getHouses 20
        let expected = actual |> Array.averageBy (fun x -> x.Price)
        test <@ averagePrice actual = expected  @>

    [<Fact>]
    let ``Exercise 4-3: Calculate prices above certain price`` () =
        let actual = getHouses 20
        let expected = actual |> Array.filter (fun x -> x.Price > 250_000m)
        test <@ abovePrice actual 250_000m = expected  @>
        
    [<Fact>]
    let ``Exercise 4-4: Returns tuple of House * SchoolDistance and excludes houses outside of school distance`` () =

        let house1 = { Address = "address 1"; Price = 100_000m }
        let house2 = { Address = "address 12"; Price = 100_000m }
        let house3 = { Address = "address 3"; Price = 100_000m }
        
        let houses = [house1; house2; house3]
        let expected = seq [house1, 13; house2, 13]
        
        let actual =
            houses
            |> schoolDistances
            
        test <@ actual = expected @>
        
        

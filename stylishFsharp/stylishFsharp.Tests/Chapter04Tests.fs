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
    let ``Exercise 4-4: Returns sequence of tuples (House * SchoolDistance) and excludes houses outside of school distance`` () =

        let house1 = { Address = "address 1"; Price = 100_000m }
        let house2 = { Address = "address 12"; Price = 100_000m }
        let house3 = { Address = "address 3"; Price = 100_000m }
        
        let houses = [house1; house2; house3]
        let expected = seq [house1, 13; house2, 13] |> Seq.toList
        
        let actual =
            houses
            |> schoolDistances
            
        test <@ actual |> Seq.toList = expected @>
               
    [<Fact>]
    let ``Exercise 4-5 (modified): Get houses with price larger 100 grand`` () =
        let house1 = { Address = "address 1"; Price = 99_999m }
        let house2 = { Address = "address 12"; Price = 100_000m }
        let house3 = { Address = "address 3"; Price = 100_001m }
        let houses = [house1; house2; house3]
        
        let actual = houses |> getHousesAbove100Grand
        
        test <@ actual = [house3] @>

    [<Fact>]
    let ``Exercise 4-6 (modified): Get houses with price larger 100 grand and sorted by price`` () =
        let house1 = { Address = "address 1"; Price = 99_999m }
        let house2 = { Address = "address 2"; Price = 100_000m }
        let house3 = { Address = "address 3"; Price = 100_001m }
        let house4 = { Address = "address 4"; Price = 200_000m }
        let house5 = { Address = "address 5"; Price = 150_000m }
        let houses = [house1; house2; house3; house4; house5]
        
        let actual = houses |> getHousesAbove100Grand
        
        test <@ actual = [house3; house5; house4] @>

    [<Fact>]
    let ``Exercise 4-7 (modified): Average price of houses above 200_000`` () =
        let house1 = { Address = "address 1"; Price = 99_999m }
        let house2 = { Address = "address 2"; Price = 100_000m }
        let house3 = { Address = "address 3"; Price = 100_001m }
        let house4 = { Address = "address 4"; Price = 250_000m }
        let house5 = { Address = "address 5"; Price = 250_000m }
        let house6 = { Address = "address 6"; Price = 350_000m }
        let house7 = { Address = "address 7"; Price = 560_000m }
        let houses = [house1; house2; house3; house4; house5; house6; house7]
        
        let actual = houses |> getAverageOfHouseAbove200grand
        
        test <@ actual = 352_500m @>

    [<Fact>]
    let ``Exercise 4-8 (modified): 1st house that costs less than 100_000 and has school distance`` () =
        let house1 = { Address = "address x 1"; Price = 99_999m }
        let house2 = { Address = "address 2"; Price = 100_000m }
        let house3 = { Address = "address 13"; Price = 50_001m }
        let house4 = { Address = "address 14"; Price = 250_000m }
        let house5 = { Address = "address 15"; Price = 50_000m }
        let house6 = { Address = "address 6"; Price = 350_000m }
        let house7 = { Address = "address 7"; Price = 560_000m }
        let houses = [house1; house2; house3; house4; house5; house6; house7]
        
        let actual = houses |> getFirstHouseUnder100grandAndWithSchoolDistance
        
        test <@ actual = (house3, 13) @>


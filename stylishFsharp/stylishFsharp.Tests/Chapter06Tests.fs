module Chapter06Tests

open System
open Xunit
open Swensen.Unquote

open Chapter06

[<Fact>]
let ``Exercise 6-1: Pattern matching on records with DUs 1/x Standard`` () =
    let inputStandard = { ReadingDate = DateTime(2020, 1, 1); MeterValue = Standard 4223 }
    let actual =
        inputStandard
        |> formatReading
    let expected = "Your reading on: 2020-01-01 was 0004223"
    
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 6-1: Pattern matching on records with DUs 2/x Economy`` () =
    let inputEconomy = { ReadingDate = DateTime(2020, 1, 1); MeterValue = Economy(Day=123, Night=22) }
    let actual =
        inputEconomy
        |> formatReading
    let expected = "Your reading on: 2020-01-01: Day: 0000123 Night: 0000022"
    
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 6-2: Record pattern matching and loops - Original implementation`` () =
    let fruits = [ "Apples", 3; "Oranges", 4; "Bananas", 2 ]
    let actual =
        fruits
        |> getFruits
    let expected = ["There are 3 Apples"; "There are 4 Oranges"; "There are 2 Bananas"]
    
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 6-2: Record pattern matching and loops - Using records`` () =
    let fruits = [ "Apples", 3; "Oranges", 4; "Bananas", 2 ]
    let fruitBatches =
        fruits
        |> fruitListToFruitBatchList
    let actual =
        fruitBatches
        |> getFruitsFromBatches
    let expected = ["There are 3 Apples"; "There are 4 Oranges"; "There are 2 Bananas"]
    
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 6-3: Zip+4 Codes and Partial Active Patterns`` () =
    let zipCodes = [
        "90210"
        "94043"
        "94043-0138"
        "10013"
        "90210-3124"
        "1OO13"
    ]
    
    let actual =
        zipCodes
        |> checkZipcodes
    
    let expected = [
        "A normal zip code: 90210"
        "A normal zip code: 94043"
        "A Zip+4 code: prefix: 94043-0138, suffix: 94043"
        "A normal zip code: 10013"
        "A Zip+4 code: prefix: 90210-3124, suffix: 90210"
        "Not a zip code: 1OO13"]
    
    test <@ actual = expected @>
    
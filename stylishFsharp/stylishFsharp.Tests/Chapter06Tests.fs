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

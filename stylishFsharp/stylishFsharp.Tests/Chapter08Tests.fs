module Chapter08Tests

open System.Drawing
open Xunit
open Swensen.Unquote
open Chapter08

[<Fact>]
let ``Exercise 8-1: A simple class`` () =
    let grayScale = GrayScale(255uy, 255uy, 255uy)
    let actual = grayScale.Level
    let expected = 255uy
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 8-2: Secondary Constructor`` () =
    let grayScale = GrayScale(Color.Brown)
    let actual = grayScale.Level
    let expected = 83uy
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 8-3: Overrides ToString`` () =
    let grayScale = GrayScale(Color.Brown)
    let actual = grayScale.ToString()
    let expected = "GrayScale(83)"
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 8-4: Equality 1/3 Orange is Orange`` () =
    let orangeByName = GrayScale(Color.Orange)
    let orangeByRGB = GrayScale(0xFFuy, 0xA5uy, 0x00uy)
    test <@ orangeByName = orangeByRGB @>
    
[<Fact>]
let ``Exercise 8-4: Equality 2/3 Orange is not Blue`` () =
    let orange = GrayScale(Color.Orange)
    let blue = GrayScale(Color.Blue)
    test <@ orange <> blue @>

[<Fact>]
let ``Exercise 8-4: Equality 3/3 Comparing similar colors`` () =
    let orange = GrayScale(0xFFuy, 0xA5uy, 0x00uy)
    let otherColor = GrayScale(0xFFuy, 0xA5uy, 0x01uy) // <- Note: `0x01uy` != `0x00uy` 
    
    // This returns true, because we lose accuracy during rounding! 
    test <@ orange = otherColor @>
    
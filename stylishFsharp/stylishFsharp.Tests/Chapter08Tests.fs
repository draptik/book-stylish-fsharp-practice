module Chapter08Tests

open Xunit
open Swensen.Unquote
open Chapter08

[<Fact>]
let ``Exercise 8-1: A simple class`` () =
    let grayScale = GrayScale(255uy, 255uy, 255uy)
    let actual = grayScale.Level
    let expected = 255uy
    test <@ actual = expected @>
    
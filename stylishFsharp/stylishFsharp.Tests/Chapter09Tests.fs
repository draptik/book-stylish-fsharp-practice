module Chapter09Tests

open Xunit
open Swensen.Unquote
open Chapter09

[<Fact>]
let ``Exercise 9-1: Functions as arguments - 1/2 pass multiply function to applyAndPrint`` () =
    let multiply a b = a * b
    let actual = applyAndPrint multiply 2 3
    test <@ actual = 6 @>

[<Fact>]
let ``Exercise 9-1: Functions as arguments - 2/2 subtract without named function`` () =
    let actual = applyAndPrint (fun x y -> x - y) 10 3
    let actual' = applyAndPrint (-) 10 3 // <- shorter syntax alternative passing the operator into `applyAndPrint`
    test <@ actual = 7 @>
    test <@ actual' = 7 @>

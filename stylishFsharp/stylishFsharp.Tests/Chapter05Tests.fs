module Chapter05Tests

open Xunit
open Swensen.Unquote

open Chapter05

[<Fact>]
let ``Exercise 5-1: Clipping a sequence`` () =
    let actual =
        seq [| 1.0; 2.3; 11.1; -5. |]
        |> clip 10.
        |> Seq.toList
    let expected = [ 1.0; 2.3; 10.; -5. ]
    
    test <@ actual = expected @>
    


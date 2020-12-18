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

[<Fact>]
let ``Exercise 9-2: Functions returning functions (with hidden state inside the function!) - 1/2 counter`` () =
    let counterFcn1 = counter 0
    let counterFcn2 = counter 100

    test <@ counterFcn1() = 0 @>
    test <@ counterFcn1() = 1 @>
    test <@ counterFcn1() = 2 @>
    test <@ counterFcn1() = 3 @>
    
    test <@ counterFcn2() = 100 @>
    test <@ counterFcn2() = 101 @>
    test <@ counterFcn2() = 102 @>
    test <@ counterFcn2() = 103 @>
    
[<Fact>]
let ``Exercise 9-2: Functions returning functions (with hidden state inside the function!) - 2/2 rangeCounter`` () =
    let actualFcn = rangeCounter 3 6

    test <@ actualFcn() = 3 @>
    test <@ actualFcn() = 4 @>
    test <@ actualFcn() = 5 @>
    test <@ actualFcn() = 6 @>
    test <@ actualFcn() = 3 @>
    test <@ actualFcn() = 4 @>
    test <@ actualFcn() = 5 @>

[<Fact>]
let ``Exercise 9-3: Partial application - 1/2 initial code`` () =
    let input = seq [100.; 150.; 200.]
    let actual = scale input |> Seq.toList
    let expected = [0.0; 0.5; 1.0]
    test <@ actual = expected @>

[<Fact>]
let ``Exercise 9-3: Partial application - 2/2 without lambda at end of scale' function`` () =
    let input = seq [100.; 150.; 200.]
    let actual = scale' input |> Seq.toList
    let expected = [0.0; 0.5; 1.0]
    test <@ actual = expected @>
    
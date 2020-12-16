module Chapter02Tests

open Xunit
open FsUnit.Xunit

open System
open Chapter02.MilesYards

[<Fact>]
let ``MilesYards fromMilesPointYards split miles and yards`` () =
    1.0880 |> fromMilesPointYards |> value |> should equal (1, 880)

[<Fact>]
let ``MilesYards fromMilesPointYards throws when yards fraction too large`` () =
    (fun () -> 1.1760 |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

[<Fact>]
let ``MilesYards toDecimalMiles 1 mile and 880 yards is 1.5 miles`` () =
    1.0880 |> fromMilesPointYards |> toDecimalMiles |> should equal 1.5

[<Fact>]
let ``MilesYards fromMilesPointYards throws when input is negative`` () =
    (fun () -> -1. |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

// load MilesChains here to avoid naming conflict of function `toDecimalMiles`
open Chapter02.MilesChains

[<Fact>]
let ``MilesChains fromMilesChains throws when chain too large`` () =
    (fun () -> (1, 80) |> fromMilesChains |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

[<Fact>]
let ``MilesChains fromMilesChains toDecimalValue 1 40 is 1.5 miles`` () =
    (1, 40) |> fromMilesChains |> toDecimalMiles |> should equal 1.5

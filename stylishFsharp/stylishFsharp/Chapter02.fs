module Chapter02

open System
open Xunit
open FsUnit.Xunit

// Listing 2-12
module MilesYards =
    type MilesYards =
        private MilesYards of wholeMiles : int * yards : int
    
    let fromMilesPointYards (milesPointYards : float) : MilesYards =
        let wholeMiles = milesPointYards |> floor |> int
        let fraction = milesPointYards - float(wholeMiles)
        if fraction > 0.1759 then
            raise <| ArgumentOutOfRangeException("milesPointYards",
                                                 "Fractional part must be <= 0.1759")
        let yards = fraction * 10_000. |> round |> int
        MilesYards(wholeMiles, yards)
        
    let toDecimalMiles (milesYards : MilesYards) : float =
        match milesYards with
        | MilesYards(wholeMiles, yards) ->
            (float wholeMiles) + ((float yards) / 1760.)

    let value (MilesYards (wholeMiles, yards)) = wholeMiles, yards

open MilesYards

[<Fact>]
let ``fromMilesPointYards split miles and yards`` () =
    1.0880 |> fromMilesPointYards |> value |> should equal (1, 880)

[<Fact>]
let ``toDecimalMiles 1 mile and 880 yards is 1.5 miles`` () =
    1.0880 |> fromMilesPointYards |> toDecimalMiles |> should equal 1.5

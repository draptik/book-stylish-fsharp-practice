module Chapter02

open System

module MilesYards =
    
    let private (~~) = float

    type MilesYards =
        private MilesYards of wholeMiles : int * yards : int
    
    let fromMilesPointYards (milesPointYards : float) : MilesYards =
        if milesPointYards < 0.0 then
            raise <| ArgumentOutOfRangeException("milesPointYards",
                                                 "Must be > 0.0")
            
        let wholeMiles = milesPointYards |> floor |> int
        let fraction = milesPointYards - float(wholeMiles)
        if fraction > 0.1759 then
            raise <| ArgumentOutOfRangeException("milesPointYards",
                                                 "Fractional part must be <= 0.1759")
        let yards = fraction * 10_000. |> round |> int
        MilesYards(wholeMiles, yards)
        
    let toDecimalMiles (MilesYards(wholeMiles, yards)) : float =
        ~~wholeMiles + (~~yards / 1760.)

    let value (MilesYards (wholeMiles, yards)) = wholeMiles, yards


open Xunit
open FsUnit.Xunit

open MilesYards

[<Fact>]
let ``fromMilesPointYards split miles and yards`` () =
    1.0880 |> fromMilesPointYards |> value |> should equal (1, 880)

[<Fact>]
let ``fromMilesPointYards throws when yards fraction too large`` () =
    (fun () -> 1.1760 |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

[<Fact>]
let ``toDecimalMiles 1 mile and 880 yards is 1.5 miles`` () =
    1.0880 |> fromMilesPointYards |> toDecimalMiles |> should equal 1.5

[<Fact>]
let ``fromMilesPointYards throws when input is negative`` () =
    (fun () -> -1. |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

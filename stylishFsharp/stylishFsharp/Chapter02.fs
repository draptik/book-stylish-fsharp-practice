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
    
module MilesChains =

    let private (~~) = float

    type MilesChains =
        private MilesChains of wholeMiles : int * chains : int
    
    let fromMilesChains(wholeMiles : int, chains : int) =
        if wholeMiles < 0 then
            raise <| ArgumentOutOfRangeException("wholeMiles",
                                                 "Must be >= 0")
            
        if chains < 0 || chains >= 80 then
            raise <| ArgumentOutOfRangeException("chains",
                                                 "Must be >= 0 and < 80")
        MilesChains(wholeMiles, chains)
        
    let toDecimalMiles (MilesChains(wholeMiles, chains)) =
        ~~wholeMiles + (~~chains / 80.)


open Xunit
open FsUnit.Xunit

open MilesYards
open MilesChains

[<Fact>]
let ``MilesYards fromMilesPointYards split miles and yards`` () =
    1.0880 |> fromMilesPointYards |> value |> should equal (1, 880)

[<Fact>]
let ``MilesYards fromMilesPointYards throws when yards fraction too large`` () =
    (fun () -> 1.1760 |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

[<Fact>]
let ``MilesYards toDecimalMiles 1 mile and 880 yards is 1.5 miles`` () =
    1.0880 |> fromMilesPointYards |> MilesYards.toDecimalMiles |> should equal 1.5

[<Fact>]
let ``MilesYards fromMilesPointYards throws when input is negative`` () =
    (fun () -> -1. |> fromMilesPointYards |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>


[<Fact>]
let ``MilesChains fromMilesChains throws when chain too large`` () =
    (fun () -> (1, 80) |> fromMilesChains |> ignore)
    |> should throw typeof<ArgumentOutOfRangeException>

[<Fact>]
let ``MilesChains fromMilesChains toDecimalValue 1 40 is 1.5 miles`` () =
    (1, 40) |> fromMilesChains |> MilesChains.toDecimalMiles |> should equal 1.5

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

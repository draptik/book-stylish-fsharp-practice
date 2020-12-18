module Chapter04

module Houses =
    
    type House = { Address : string; Price : decimal }
    type PriceBand = | Cheap | Medium | Expensive
    
    let getHouses count =
        let random = System.Random(Seed = 1)
        Array.init count (fun i ->
            { Address = sprintf "%i Stochastic Street" (i+1)
              Price = random.Next(50_000, 500_000) |> decimal })

    let random = System.Random(Seed = 1)
    
    let trySchoolDistance (house: House) =
        let a = house.Address
        if a.Contains("address 1") then
            Some 13
        else
            None

    let schoolDistances (houses: House seq) : (House * int) seq =
        houses
        |> Seq.choose (fun house ->
            match house |> trySchoolDistance with
            | Some distance -> Some (house, distance)
            | None -> None)
        
    let priceBand (price : decimal) =
        if price < 100_000m then
            Cheap
        else if price < 200_000m then
            Medium
        else
            Expensive

    let averagePrice houses =
        houses |> Array.averageBy (fun x -> x.Price)
        
    let abovePrice houses price =
        houses |> Array.filter (fun x -> x.Price > price)

    let getHousesAbove100Grand (houses : House list) =
        houses
        |> List.filter (fun h -> h.Price > 100_000m)
        |> List.sortBy (fun h -> h.Price)

    let getAverageOfHouseAbove200grand (houses : House list) =
        houses
        |> List.filter (fun h -> h.Price > 200_000m)
        |> List.averageBy (fun h -> h.Price)
        
module Exercise04_01 =
    open Houses
    
    let housePrices =
        getHouses 20
        |> Array.map (fun h ->
            sprintf "Address: %s - Price: %f" h.Address h.Price)

module Exercise04_05 =
    open Houses
    
    getHouses 20
    |> Array.filter (fun h -> h.Price > 100_000m)
    |> Array.iter (fun h ->
        printfn "Address: %s Price %f" h.Address h.Price)
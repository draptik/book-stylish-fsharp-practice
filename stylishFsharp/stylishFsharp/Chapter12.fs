module Chapter12

module Dummy =

    open System.Threading

    let slowFunction () =
        Thread.Sleep(100)
        99
    
    let fastFunction () =
        Thread.Sleep(10)
        99

module Exercise12_1 =
    type Transaction = { Id : int }
    
    let addTransactionsBaseline (oldTransactions : Transaction list) (newTransactions : Transaction list) =
        oldTransactions @ newTransactions
    
    let addTransactions (oldTransactions : Transaction []) (newTransactions : Transaction []) =
        Array.append oldTransactions newTransactions

module Exercise12_2 =
    type Float3 = (float * float * float)

    let private distance x1 y1 z1 x2 y2 z2 =
        let dx = x1 - x2
        let dy = y1 - y2
        let dz = z1 - z2
        dx * dx +
        dy * dy +
        dz * dy
        |> sqrt
    
    let withinRadiusBaseline (radius : float) (here : Float3) (coords : Float3[]) =
        let x1, y1, z1 = here
        coords
        |> Array.filter (fun (x2, y2, z2) ->
            distance x1 y1 z1 x2 y2 z2 <= radius)
        
    let withinRadius (radius : float) (here : Float3) (coords : Float3[]) =
        let x1, y1, z1 = here
        coords
        |> Array.Parallel.map (fun (x2, y2, z2) ->
            distance x1 y1 z1 x2 y2 z2)
        |> Array.filter (fun d -> d <= radius)
          
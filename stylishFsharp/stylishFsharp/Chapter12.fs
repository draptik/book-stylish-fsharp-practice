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
    
    let addTransactions (oldTransactions : Transaction list) (newTransactions : Transaction list) =
        oldTransactions @ newTransactions
    
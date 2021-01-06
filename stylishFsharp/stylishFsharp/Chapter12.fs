module Chapter12

module Dummy =

    open System.Threading

    let slowFunction () =
        Thread.Sleep(100)
        99
    
    let fastFunction () =
        Thread.Sleep(10)
        99

//module CaseStudy_01_CollectionTypes =
    
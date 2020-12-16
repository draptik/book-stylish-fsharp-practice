module Chapter03

type Delivery =
    | AsBilling
    | Physical of string
    | Download
    | ClickAndCollect of int
    
type BillingDetails = {
    name: string
    billing: string
    delivery: Delivery }

let tryDeliveryLabel (billingDetails : BillingDetails) =
    match billingDetails.delivery with
    | AsBilling ->
        billingDetails.billing |> Some
    | Physical address ->
        address |> Some
    | Download ->
        None
    | ClickAndCollect _ ->
        None
    |> Option.map (fun address ->
        sprintf "%s\n%s" billingDetails.name address)
    
let deliveryLabels (billingDetails : BillingDetails seq) =
    billingDetails
    |> Seq.choose tryDeliveryLabel

let collectionFor (storeId : int) (billingDetails : BillingDetails seq) =
    billingDetails
    |> Seq.choose (fun d ->
        match d.delivery with
        | ClickAndCollect id when id = storeId -> Some d
        | _ -> None)
    
let myOrder = {
    name = "Kit Easton"
    billing = "112 Fibonacci Street\nErewhon\n35813"
    delivery = AsBilling }

let hisOrder = {
    name = "John Doe"
    billing = "314 Pi Avenue\nErewhon\n15926"
    delivery = Physical "16 Planck Parkway\nErewhon\n62291" }

let herOrder = {
    name = "Jane Smith"
    billing = "9 Gravity Road\nErewhon\n80665"
    delivery = Download }

let homerOrder = { name = "Homer Simpson"; billing = "Evergreen Terrace 1"; delivery = ClickAndCollect 1 }
let lisaOrder = { name = "Lisa Simpson"; billing = "Evergreen Terrace 1"; delivery = ClickAndCollect 1 }
let margeOrder = { name = "Marge Simpson"; billing = "Evergreen Terrace 1"; delivery = ClickAndCollect 2 }

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``AsBilling has correct delivery label`` () =
    myOrder
    |> tryDeliveryLabel
    |> should equal (Some "Kit Easton\n112 Fibonacci Street\nErewhon\n35813")
    
[<Fact>]
let ``Physical has correct delivery label`` () =
    hisOrder
    |> tryDeliveryLabel
    |> should equal (Some "John Doe\n16 Planck Parkway\nErewhon\n62291")
    
[<Fact>]
let ``Download has no delivery label`` () =
    herOrder
    |> tryDeliveryLabel
    |> should equal None
    
[<Fact>]
let ``deliveryLabels creates collection`` () =
    let result =
        [myOrder; hisOrder; herOrder]
        |> deliveryLabels
        |> Seq.toList
    result |> should haveLength 2
    result |> should equal ["Kit Easton
112 Fibonacci Street
Erewhon
35813";
 "John Doe
16 Planck Parkway
Erewhon
62291"]

[<Fact>]
let ``Click and Collect works`` () =
    let storeId1 = 1
    let orders = [myOrder; hisOrder; herOrder; homerOrder; lisaOrder; margeOrder]
    orders
    |> collectionFor storeId1
    |> Seq.toList
    |> should equal [
           { name = "Homer Simpson"; billing = "Evergreen Terrace 1"; delivery = ClickAndCollect 1 }
           { name = "Lisa Simpson"; billing = "Evergreen Terrace 1"; delivery = ClickAndCollect 1 }]

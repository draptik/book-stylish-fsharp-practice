module Chapter03

type Delivery =
    | AsBilling
    | Physical of string
    | Download
    
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
    | Download -> None
    |> Option.map (fun address ->
        sprintf "%s\n%s" billingDetails.name address)
    
let deliveryLabels (billingDetails : BillingDetails seq) =
    billingDetails
    |> Seq.choose tryDeliveryLabel

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



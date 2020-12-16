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

let getNumberOfNonNullBillingAddresses (billingDetails : BillingDetails seq) =
    billingDetails
    |> Seq.choose(fun x ->
        if x.billing = null then None
        else Some 1)
    |> Seq.sum

// alternative solution to `getNumberOfNonNullBillingAddresses` from book:
let countNonNullBillingAddresses (orders : seq<BillingDetails>) =
    orders
    |> Seq.map (fun bd -> bd.billing)
    |> Seq.map Option.ofObj
    |> Seq.sumBy Option.count

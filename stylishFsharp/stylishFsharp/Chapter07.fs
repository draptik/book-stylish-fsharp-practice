module Chapter07

open System

type Position = {
    X: float32
    Y: float32
    Z: float32
    Time: DateTime
}

[<Struct>]
type PositionStruct = {
    X: float32
    Y: float32
    Z: float32
    Time: DateTime
}

let translate dx dy dz pos =
    { pos with
        PositionStruct.X = pos.X + dx
        Y = pos.Y + dy
        Z = pos.Z + dz }

type Track (name : string, artist : string) =
    member __.Name = name
    member __.Artist = artist

type TrackAsRecord = {
    Name : string
    Artist : string
}    
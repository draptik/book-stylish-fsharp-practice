module Chapter08

type GrayScale (r : byte, g : byte, b : byte) =
    member __.Level =
        (int r + int g + int b) / 3 |> byte
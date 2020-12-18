module Chapter08

type GrayScale (r : byte, g : byte, b : byte) =
    new (color : System.Drawing.Color) =
        GrayScale(color.R, color.G, color.B)
        
    member __.Level =
        (int r + int g + int b) / 3 |> byte

module Chapter08

open System
open System.Drawing

type GrayScale (r : byte, g : byte, b : byte) =

    let level = (int r + int g + int b) / 3 |> byte
    
    let eq (that : GrayScale) =
        level = that.Level
    
    new (color : Color) =
        GrayScale(color.R, color.G, color.B)
        
    member __.Level =
        level    
        
    override __.ToString () =
        sprintf "GrayScale(%i)" __.Level
            
    override this.GetHashCode () =
        hash level

    override __.Equals (thatObj) =
        match thatObj with
        | :? GrayScale as that ->
            eq that
        | _ ->
            false
            
    interface IEquatable<GrayScale> with
        member __.Equals(that : GrayScale) =
            eq that

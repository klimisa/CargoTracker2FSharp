#r "netstandard"
open System
open System.Text.RegularExpressions
type TrackingId(value: Guid) =
    member x.Value = value

let m = TrackingId (Guid.NewGuid())
m.Value

let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"

type UnLocode(value: string) =
    member x.Value = value
    
    
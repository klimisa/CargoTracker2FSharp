namespace FSharp.Domain.Shipping.Cargo

open System

type TrackingId(value: Guid) =
    member x.Value = value
    
type HandlingType =
    | Load = 0
    | Unload = 1
    | Receive = 2
    | Claim = 3
    | Customs = 4    
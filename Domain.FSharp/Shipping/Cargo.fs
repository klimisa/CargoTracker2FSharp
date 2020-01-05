namespace FSharp.Domain.Shipping.Cargo

open System
open System.Text.RegularExpressions

type TrackingId(value: Guid) =
    member this.Value = value

type HandlingType =
    | Load = 0
    | Unload = 1
    | Receive = 2
    | Claim = 3
    | Customs = 4

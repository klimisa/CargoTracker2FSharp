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

type UnLocode(value: string) =

    do
        if value = null then raise (ArgumentNullException("value"))


    do
        let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
        let matches = pattern.Match value
        if not matches.Success then raise (ArgumentException("Provided value is not a valid UnLocode", "value"))

    member val Value = value

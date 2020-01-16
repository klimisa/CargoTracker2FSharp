namespace FSharp.Domain.Shipping.Cargo

open System

type VoyageNumber(value: string) =

    do
        if isNull value then raise (ArgumentNullException("value"))

    member val Value = value

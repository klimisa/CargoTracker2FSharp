namespace Domain.Shipping.Voyage

open System

[<AllowNullLiteral>]
type VoyageNumber(value: string) =

    do
        if isNull value then raise (ArgumentNullException("value"))

    member val Value = value

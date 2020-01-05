namespace FSharp.Domain.Shipping.Location

open System
open System.Text.RegularExpressions

type UnLocode(value: string) =

    do
        if value = null then raise (ArgumentNullException("value"))

    do
        let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
        let matches = pattern.Match value
        if not matches.Success then raise (ArgumentException("Provided value is not a valid UnLocode", "value"))

    member val Value = value

type Location(unlocode: UnLocode, name: string) =

    do
        if String.IsNullOrWhiteSpace(name) then raise (ArgumentException("Provided name is not valid", "name"))

    member val UnLocode = unlocode
    member val Name = name

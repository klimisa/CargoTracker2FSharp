namespace Domain.Shipping.Location

open System
open System
open System.Text.RegularExpressions

type UnLocode = private UnLocode of string

module UnLocode =
    let value (UnLocode str) = str
    let create value =
        let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
        let matches = pattern.Match value
        if String.IsNullOrWhiteSpace value then Error "Unlocode cannot be empty or null."
        else if not matches.Success then Error "Provided value is not a valid UnLocode"
        else Ok (UnLocode value)

type Location =
    { UnLocode: UnLocode
      Name: string }

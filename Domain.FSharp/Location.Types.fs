namespace Domain.Location

open System
open System.Text.RegularExpressions

type UnLocode = private UnLocode of string

type Location =
    { UnLocode: UnLocode
      Name: string }

module UnLocode =
    let value (UnLocode str) = str

    let create value =
        let pattern = "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
        if String.IsNullOrWhiteSpace value then Error "Unlocode cannot be empty or null."
        else if Regex.IsMatch(value, pattern) then Ok(UnLocode value)
        else Error "Provided value is not a valid UnLocode"

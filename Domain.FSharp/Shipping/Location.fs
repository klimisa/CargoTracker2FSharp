namespace Domain.Shipping.Location

open System
open System.Text.RegularExpressions

[<AllowNullLiteral>]
type UnLocode(value: string) =

    do
        if isNull value then raise <| ArgumentNullException "value"


    do
        let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
        let matches = pattern.Match value
        if not matches.Success then raise <| ArgumentException("Provided value is not a valid UnLocode", "value")

    member val Value = value
    override this.GetHashCode() =
        hash (value)
    override this.Equals(other) =
        match other with
        | :? UnLocode as o -> (value) = (o.Value)
        | _ -> false

[<AllowNullLiteral>]
type Location(unlocode: UnLocode, name: string) =

    do
        if isNull unlocode then raise <| ArgumentNullException "Provided name is not valid"

    do
        if String.IsNullOrWhiteSpace(name) then raise <| ArgumentException("Provided name is not valid", "name")

    member val UnLocode = unlocode
    member val Name = name
    override this.GetHashCode() =
        hash (unlocode, name)
    override this.Equals(other) =
        match other with
        | :? Location as o -> (unlocode, name) = (o.UnLocode, o.Name)
        | _ -> false    

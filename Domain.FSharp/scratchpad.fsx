open System.Collections.Generic
open System.Collections.ObjectModel

#r "netstandard"

open System
open System.Text.RegularExpressions

type TrackingId(value: Guid) =
    member x.Value = value

let m = TrackingId(Guid.NewGuid())

m.Value

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
    
let unlocode = "xa234"
let pattern = Regex "[a-zA-Z]{2}[a-zA-Z2-9]{3}"
let matches = (pattern.Match unlocode).Success

let u = UnLocode(null)

let l = Location(u, "Myname")
l.UnLocode.Value

type VoyageNumber(value: string) =

    do
        if isNull value then raise (ArgumentNullException("value"))

    member val Value = value
    
type Leg(voyage: VoyageNumber, loadLocation: UnLocode, unloadLocation: UnLocode, loadTime: DateTime, unloadTime: DateTime) =

    do
        if loadTime >= unloadTime then raise <| ArgumentException "unloadTime should be later than loadTime"

    member val VoyageNumber = voyage
    member val LoadLocation = loadLocation
    member val UnloadLocation = unloadLocation
    member val LoadTime = loadTime
    member val UnloadTime = unloadTime
    
type Itinerary(legs: IList<Leg>) =
    
    member val Legs = ReadOnlyCollection<Leg>(legs) :> IReadOnlyCollection<Leg>
    



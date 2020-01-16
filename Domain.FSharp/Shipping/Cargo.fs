namespace FSharp.Domain.Shipping.Cargo

open FSharp.Domain.Shipping.Location
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Linq

type TrackingId(value: Guid) =
    member this.Value = value

type HandlingType =
    | Load = 0
    | Unload = 1
    | Receive = 2
    | Claim = 3
    | Customs = 4


type Leg(voyage: VoyageNumber, loadLocation: UnLocode, unloadLocation: UnLocode, loadTime: DateTime, unloadTime: DateTime) =

    do
        if loadTime >= unloadTime then raise <| ArgumentException "unloadTime should be later than loadTime"

    member val Voyage = voyage
    member val LoadLocation = loadLocation
    member val UnloadLocation = unloadLocation
    member val LoadTime = loadTime
    member val UnloadTime = unloadTime

type Itinerary(legs: IList<Leg>) =

    do
        if isNull legs then raise <| ArgumentNullException "legs"

    do
        if legs.Count = 0 then raise <| ArgumentException "legs cannot be empty"

    member val Legs: IReadOnlyCollection<Leg> = ReadOnlyCollection<Leg>(legs) :> IReadOnlyCollection<Leg>
    member this.FirstLoadLocation = this.Legs.First().LoadLocation
    member this.FirstVoyage = this.Legs.First().Voyage
    member this.LastUnloadLocation = this.Legs.Last().UnloadLocation
    member this.FinalArrivalDate = this.Legs.Last().UnloadTime

type RouteSpecification(origin: UnLocode, destination: UnLocode, arrivalDeadline: DateTime) =

    do
        if origin = destination then raise <| ArgumentNullException "value"

    member val Origin = origin
    member val Destination = destination
    member val ArrivalDeadline = arrivalDeadline

//TODO: missing IsSatisfiedBy method

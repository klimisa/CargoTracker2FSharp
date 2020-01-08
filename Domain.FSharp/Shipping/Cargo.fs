namespace FSharp.Domain.Shipping.Cargo

open FSharp.Domain.Shipping.Location
open System

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

    member val VoyageNumber = voyage
    member val LoadLocation = loadLocation
    member val UnloadLocation = unloadLocation
    member val LoadTime = loadTime
    member val UnloadTime = unloadTime

type RouteSpecification(origin: UnLocode, destination: UnLocode, arrivalDeadline: DateTime) =

    do
        if origin = destination then raise <| ArgumentNullException "value"

    member val Origin = origin
    member val Destination = destination
    member val ArrivalDeadline = arrivalDeadline
    
    //TODO: missing IsSatisfiedBy method

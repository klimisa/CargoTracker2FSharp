namespace FSharp.Domain.Shipping.Cargo

open FSharp.Domain.Shipping.Location
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Linq

type HandlingType =
    | Load = 0
    | Unload = 1
    | Receive = 2
    | Claim = 3
    | Customs = 4

type RoutingStatus =
    | NotRouted = 0
    | Routed = 1
    | MisRoute = 2

type TransportStatus =
    | NotReceived = 0
    | InPort = 1
    | OnBoardVessel = 2
    | Claimed = 3
    | Unknown = 4

type TrackingId(value: Guid) =
    member this.Value = value

type Leg(voyage: VoyageNumber, loadLocation: UnLocode, unloadLocation: UnLocode, loadTime: DateTime, unloadTime: DateTime) =

    do
        if loadTime >= unloadTime then raise <| ArgumentException "unloadTime should be later than loadTime"

    member val Voyage = voyage
    member val LoadLocation = loadLocation
    member val UnloadLocation = unloadLocation
    member val LoadTime = loadTime
    member val UnloadTime = unloadTime

type HandlingEvent(trackingId: TrackingId, type': HandlingType, location: UnLocode, voyage: VoyageNumber, completed: DateTime, registered: DateTime) =

    do
        if (type' = HandlingType.Load || type' = HandlingType.Unload) then
            raise <| InvalidOperationException "loading/unloading events need a voyage"

    member val TrackingId = trackingId
    member val Location = location
    member val Type = type'
    member val Voyage = voyage
    member val Completed = completed
    member val Registered = registered

type HandlingActivity(type': HandlingType, location: UnLocode, voyage: VoyageNumber) =
    member val Location = location
    member val Type = type'
    member val Voyage = voyage

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

    member this.NextOf(location: UnLocode) =
        // TODO: This is set to null in c#
        let mutable next = this.Legs.First()
        let mutable currentFound = false
        let mutable breakLoop = false
        let en = this.Legs.GetEnumerator()
        while en.MoveNext() && not breakLoop do
            if currentFound then
                next <- en.Current
                breakLoop <- true
            if en.Current.LoadLocation = location || en.Current.UnloadLocation = location then currentFound <- true
        next

    member this.Of(location: UnLocode) =
        this.Legs.SingleOrDefault(fun l -> l.UnloadLocation = location || l.LoadLocation = location)
    member this.IsExpected(event: HandlingEvent) =
        match event.Type with
        | HandlingType.Receive -> this.FirstLoadLocation = event.Location
        | HandlingType.Load -> this.Legs.Any(fun l -> l.LoadLocation = event.Location)
        | HandlingType.Unload -> this.Legs.Any(fun l -> l.UnloadLocation = event.Location)
        | HandlingType.Claim
        | HandlingType.Customs -> this.LastUnloadLocation = event.Location
        | _ -> false

type RouteSpecification(origin: UnLocode, destination: UnLocode, arrivalDeadline: DateTime) =
    do
        if origin = destination then raise <| ArgumentNullException "value"

    member val Origin = origin
    member val Destination = destination
    member val ArrivalDeadline = arrivalDeadline
    member this.IsSatisfiedBy(itinerary: Itinerary) =
        itinerary.FirstLoadLocation = this.Origin && itinerary.LastUnloadLocation = this.Destination
        && itinerary.FinalArrivalDate <= this.ArrivalDeadline

type Delivery(routeSpec: RouteSpecification, itinerary: Itinerary, lastHandlingEvent: HandlingType) as this =
    let _calcTransportStatus event =
        match event with
        | HandlingType.Load ->  this.TransportStatus = TransportStatus.OnBoardVessel
    member val RouteSpec = routeSpec
    member val Itinerary = itinerary
    member val LastHandlingEvent = lastHandlingEvent
    member val TransportStatus with get(), private set()
    
    
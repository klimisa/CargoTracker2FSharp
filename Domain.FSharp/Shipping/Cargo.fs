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
    | MisRouted = 2

type TransportStatus =
    | NotReceived = 0
    | InPort = 1
    | OnBoardVessel = 2
    | Claimed = 3
    | Unknown = 4

type TrackingId(value: Guid) =
    member this.Value = value

[<AllowNullLiteral>]
type Leg(voyage: VoyageNumber, loadLocation: UnLocode, unloadLocation: UnLocode, loadTime: DateTime, unloadTime: DateTime) =

    do
        if isNull voyage then raise <| ArgumentNullException "voyage"
        if isNull loadLocation then raise <| ArgumentNullException "loadLocation"
        if isNull unloadLocation then raise <| ArgumentNullException "unloadLocation"
        if loadTime >= unloadTime then raise <| ArgumentException "unloadTime should be later than loadTime"

    member val Voyage = voyage
    member val LoadLocation = loadLocation
    member val UnloadLocation = unloadLocation
    member val LoadTime = loadTime
    member val UnloadTime = unloadTime

[<AllowNullLiteral>]
type HandlingEvent(trackingId: TrackingId, type': HandlingType, location: UnLocode, voyage: VoyageNumber, completed: DateTime, registered: DateTime) =

    do
        if isNull location then raise <| ArgumentNullException "location"
        if (type' = HandlingType.Load || type' = HandlingType.Unload) && isNull voyage then
            raise <| InvalidOperationException "loading/unloading events need a voyage"

    member val TrackingId = trackingId
    member val Location = location
    member val Type = type'
    member val Voyage = voyage
    member val Completed = completed
    member val Registered = registered

[<AllowNullLiteral>]
type HandlingActivity(type': HandlingType, location: UnLocode, voyage: VoyageNumber) =

    do
        if isNull location then raise <| ArgumentNullException "location"
        if type' = HandlingType.Load && isNull voyage then
            raise <| InvalidOperationException "a load activity needs a voyage"
        if type' = HandlingType.Unload && isNull voyage then
            raise <| InvalidOperationException "an unload activity needs a voyage"

    member val Location = location
    member val Type = type'
    member val Voyage = voyage

[<AllowNullLiteral>]
type Itinerary(legs: IList<Leg>) =

    do
        if isNull legs then raise <| ArgumentNullException "legs"
        if legs.Count = 0 then raise <| ArgumentException "legs cannot be empty"

    member val Legs: IReadOnlyCollection<Leg> = ReadOnlyCollection<Leg>(legs) :> IReadOnlyCollection<Leg>
    member this.FirstLoadLocation = this.Legs.First().LoadLocation
    member this.FirstVoyage = this.Legs.First().Voyage
    member this.LastUnloadLocation = this.Legs.Last().UnloadLocation
    member this.FinalArrivalDate = this.Legs.Last().UnloadTime

    member this.NextOf(location: UnLocode) =
        let mutable next = null
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

[<AllowNullLiteral>]
type RouteSpecification(origin: UnLocode, destination: UnLocode, arrivalDeadline: DateTime) =

    do
        if isNull origin then raise <| ArgumentNullException "origin"
        if isNull destination then raise <| ArgumentNullException "destination"
        if origin = destination then raise <| InvalidOperationException "Provided origin and destination are the same"

    member val Origin = origin
    member val Destination = destination
    member val ArrivalDeadline = arrivalDeadline
    member this.IsSatisfiedBy(itinerary: Itinerary) =
        itinerary.FirstLoadLocation = this.Origin && itinerary.LastUnloadLocation = this.Destination
        && itinerary.FinalArrivalDate <= this.ArrivalDeadline

[<AllowNullLiteral>]
type Delivery(routeSpec: RouteSpecification, itinerary: Itinerary, lastHandlingEvent: HandlingEvent) as self =
    [<DefaultValue>]
    val mutable private _transportStatus: TransportStatus
    [<DefaultValue>]
    val mutable private _lastKnownLocation: UnLocode
    [<DefaultValue>]
    val mutable private _currentVoyage: VoyageNumber
    [<DefaultValue>]
    val mutable private _nextExpectedHandlingActivity: HandlingActivity
    [<DefaultValue>]
    val mutable private _routingStatus: RoutingStatus
    [<DefaultValue>]
    val mutable private _isUnloadedAtDestination: bool
    [<DefaultValue>]
    val mutable private _isMishandled: bool

    do
        if isNull routeSpec then raise <| ArgumentNullException "routeSpec"
        if isNull itinerary then raise <| ArgumentNullException "itinerary"

    member val RouteSpec = routeSpec
    member val Itinerary = itinerary
    member val LastHandlingEvent = lastHandlingEvent

    do
        self._calcTransportStatus lastHandlingEvent
        self._calcLastKnownLocation lastHandlingEvent
        self._calcCurrentVoyage lastHandlingEvent
        self._calcNextExpectedHandlingActivity routeSpec itinerary lastHandlingEvent
        self._calcIsUnloadedAtDestination routeSpec lastHandlingEvent
        self._calcRoutingStatus routeSpec itinerary
        self._calcIsMishandled itinerary lastHandlingEvent

    member this.TransportStatus
        with get () = this._transportStatus
        and private set (value) = this._transportStatus <- value

    member this.RoutingStatus
        with get () = this._routingStatus
        and private set (value) = this._routingStatus <- value

    member this.LastKnownLocation
        with get () = this._lastKnownLocation
        and private set (value) = this._lastKnownLocation <- value

    member this.CurrentVoyage
        with get () = this._currentVoyage
        and private set (value) = this._currentVoyage <- value

    member this.NextExpectedHandlingActivity
        with get () = this._nextExpectedHandlingActivity
        and private set (value) = this._nextExpectedHandlingActivity <- value

    member this.IsUnloadedAtDestination
        with get () = this._isUnloadedAtDestination
        and private set (value) = this._isUnloadedAtDestination <- value

    member this.IsMishandled
        with get () = this._isMishandled
        and private set (value) = this._isMishandled <- value

    member private this._calcTransportStatus (event: HandlingEvent) =
        if isNull event then
            this.TransportStatus <- TransportStatus.NotReceived
        else
            match event.Type with
            | HandlingType.Load -> this.TransportStatus <- TransportStatus.OnBoardVessel
            | HandlingType.Unload
            | HandlingType.Receive
            | HandlingType.Customs -> this.TransportStatus <- TransportStatus.InPort
            | HandlingType.Claim -> this.TransportStatus <- TransportStatus.Claimed
            | _ -> raise <| ArgumentOutOfRangeException "event.Type"

    member private this._calcRoutingStatus (routeSpec: RouteSpecification) (itinerary: Itinerary) =
        if isNull itinerary then this.RoutingStatus <- RoutingStatus.NotRouted
        else if routeSpec.IsSatisfiedBy itinerary then this.RoutingStatus <- RoutingStatus.Routed
        else this.RoutingStatus <- RoutingStatus.MisRouted

    member private this._calcLastKnownLocation (event: HandlingEvent) =
        if isNull event then this.LastKnownLocation <- event.Location

    member private this._calcCurrentVoyage (event: HandlingEvent) =
        if isNull event then this.CurrentVoyage <- event.Voyage

    member private this._calcNextExpectedHandlingActivity (routeSpec: RouteSpecification) (itinerary: Itinerary)
           (event: HandlingEvent) =
        if isNull itinerary then
            this.NextExpectedHandlingActivity <- null
        else
            this._calcRoutingStatus routeSpec itinerary
            if this.RoutingStatus = RoutingStatus.MisRouted then
                this.NextExpectedHandlingActivity <- null
            else if isNull event then
                this.NextExpectedHandlingActivity <-
                    HandlingActivity(HandlingType.Receive, itinerary.FirstLoadLocation, null)
            else
                match event.Type with
                | HandlingType.Receive ->
                    this.NextExpectedHandlingActivity <-
                        HandlingActivity(HandlingType.Load, itinerary.FirstLoadLocation, itinerary.FirstVoyage)
                | HandlingType.Load ->
                    let leg = itinerary.Of event.Location
                    this.NextExpectedHandlingActivity <-
                        HandlingActivity(HandlingType.Unload, leg.UnloadLocation, leg.Voyage)
                | HandlingType.Unload ->
                    if event.Location = itinerary.LastUnloadLocation then
                        this.NextExpectedHandlingActivity <-
                            HandlingActivity(HandlingType.Customs, itinerary.LastUnloadLocation, null)
                    else
                        let nextLeg = itinerary.NextOf event.Location
                        this.NextExpectedHandlingActivity <-
                            HandlingActivity(HandlingType.Load, nextLeg.LoadLocation, nextLeg.Voyage)
                | HandlingType.Customs ->
                    this.NextExpectedHandlingActivity <-
                        HandlingActivity(HandlingType.Claim, itinerary.LastUnloadLocation, null)
                | _ -> this.NextExpectedHandlingActivity <- null

    member private this._calcIsUnloadedAtDestination (routeSpec: RouteSpecification) (event: HandlingEvent) =
        if isNull event then this.IsUnloadedAtDestination <- false
        else this.IsUnloadedAtDestination <- routeSpec.Destination = event.Location

    member private this._calcIsMishandled (itinerary: Itinerary) (event: HandlingEvent) =
        if isNull event || isNull itinerary then this.IsMishandled <- false
        else this.IsMishandled <- itinerary.IsExpected event

[<AllowNullLiteral>]
type Cargo(trackingId: TrackingId, routeSpec: RouteSpecification) =
    [<DefaultValue>]
    val mutable private _itinerary: Itinerary

    [<DefaultValue>]
    val mutable private _lastHandlingEvent: HandlingEvent

    do
        if isNull routeSpec then raise <| ArgumentNullException "routeSpec"

    let mutable _trackingId: TrackingId = trackingId
    let mutable _routeSpec: RouteSpecification = routeSpec
    let mutable _delivery: Delivery = Delivery(routeSpec, null, null)

    // rehydration ctor
    new(trackingId: TrackingId, routeSpec: RouteSpecification, itinerary: Itinerary, lastHandlingEvent: HandlingEvent) as self =
        Cargo(trackingId, routeSpec, itinerary, lastHandlingEvent)
        then
            self.TrackingId <- trackingId
            self.RouteSpec <- routeSpec
            self.Itinerary <- itinerary
            self.LastHandlingEvent <- lastHandlingEvent
            self.Delivery <- Delivery(routeSpec, itinerary, lastHandlingEvent)

    member this.TrackingId
        with get () = _trackingId
        and private set (value) = _trackingId <- value

    member this.RouteSpec
        with get () = _routeSpec
        and private set (value) = _routeSpec <- value

    member this.Delivery
        with get () = _delivery
        and private set (value) = _delivery <- value

    member this.Itinerary
        with get () = this._itinerary
        and private set (value) = this._itinerary <- value

    member this.LastHandlingEvent
        with get () = this._lastHandlingEvent
        and private set (value) = this._lastHandlingEvent <- value

    member this.AssignToItinerary(itinerary: Itinerary) =
        if isNull itinerary then raise <| ArgumentNullException "itinerary"
        this.Itinerary <- itinerary
        this.Delivery <- Delivery(this.RouteSpec, this.Itinerary, this.LastHandlingEvent)
    // TODO: add events
    member this.ChangeRoute(routeSpec: RouteSpecification) =
        if isNull routeSpec then raise <| ArgumentNullException "routeSpec"
        this.RouteSpec <- routeSpec
        this.Delivery <- Delivery(this.RouteSpec, this.Itinerary, this.LastHandlingEvent)
    // TODO: add events
    member this.RegisterHandlingEvent(event: HandlingEvent) =
        if isNull event then raise <| ArgumentNullException "event"
        this.LastHandlingEvent <- event
        this.Delivery <- Delivery(this.RouteSpec, this.Itinerary, this.LastHandlingEvent)
// TODO: add events

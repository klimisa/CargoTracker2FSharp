namespace Domain.FSharp.Shipping.Events

open System
open FSharp.Domain.Shipping.Cargo

type NewBooked(trackingId: TrackingId, routeSpec: RouteSpecification) =
    do
        if isNull routeSpec then raise <| ArgumentNullException "trackingId"
    
    member val TrackingId = trackingId
    member val RouteSpec = routeSpec
    
type AssignedToItinerary(trackingId: TrackingId, itinerary: Itinerary) =
    do
        if isNull itinerary then raise <| ArgumentNullException "itinerary"
    
    member val TrackingId = trackingId
    member val Itinerary = itinerary
    
type DeliveryStateChanged(trackingId: TrackingId, delivery: Delivery) =
    do
        if isNull delivery then raise <| ArgumentNullException "delivery"
    
    member val TrackingId = trackingId
    member val Delivery = delivery
    
type HandlingEventRegistered(event: HandlingEvent) =
    do
        if isNull event then raise <| ArgumentNullException "event"
    
    member val HandlingEvent = event             
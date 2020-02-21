namespace Domain.Shipping

open System

open Domain.Location
open Domain.Voyage

type Undefined = exn
type TrackingId = Undefined

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

type Leg =
    { Voyage: VoyageNumber
      LoadLocation: UnLocode
      UnloadLocation: UnLocode
      LoadTime: DateTime
      UnloadTime: DateTime }

type HandlingEvent =
    { TrackingId: TrackingId
      Location: UnLocode
      Type: HandlingType
      Voyage: VoyageNumber
      Completed: DateTime
      Registered: DateTime }

type HandlingActivity =
    { Location: UnLocode
      Type: HandlingType
      Voyage: VoyageNumber }

type Cargo = {
    Id: TrackingId
    Origin: Location
    Destination: Location
    ArrivalDeadline: DateTime
}

// Location
type UnvalidatedLocation = UnvalidatedLocation of string
type ValidatedLocation = private ValidatedLocation of Location
type LocationValidationService = UnvalidatedLocation -> ValidatedLocation option

// Arrival Deadline
type UnvalidatedArrivalDeadline = UnvalidatedArrivalDeadline of string
type ValidatedArrivalDeadline = private ValidatedArrivalDeadline of DateTime
type ArrivalDeadlineValidationService = UnvalidatedArrivalDeadline -> ValidatedArrivalDeadline option

type UnvalidatedBookCargo = {
    UnvalidatedOrigin: UnvalidatedLocation
    UnvalidatedDestination: UnvalidatedLocation
    UnvalidatedArrivalDeadline: UnvalidatedArrivalDeadline
}

type ValidatedBookCargo = {
    ValidatedOrigin: ValidatedLocation
    ValidatedDestination: ValidatedLocation
    ValidatedArrivalDeadline: ValidatedArrivalDeadline
}

type CargoBooked = {
    Id: TrackingId
    Origin: Location
    Destination: Location
    ArrivalDeadline: DateTime
}

type ValidationError = {
    FieldName: string
    ErrorDescription: string
}

type BookCargoError = 
   | ValidationError of ValidationError list

type BookCargo = UnvalidatedBookCargo -> Result<CargoBooked, BookCargoError>

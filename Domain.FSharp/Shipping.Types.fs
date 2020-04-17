namespace Domain.Shipping

open System

open Domain.Location
open Domain.Voyage

type Undefined = exn
type TrackingId = Undefined

type RoutingStatus =
    | NotRouted = 0
    | Routed = 1
    | MisRouted = 2
    
// ----------------------
// Input Data
// ----------------------
type UnvalidatedLocation = UnvalidatedLocation of string
type UnvalidatedArrivalDeadline = UnvalidatedArrivalDeadline of string
type UnvalidatedBookCargo = {
    TrackingId: string
    UnvalidatedOrigin: UnvalidatedLocation
    UnvalidatedDestination: UnvalidatedLocation
    UnvalidatedArrivalDeadline: UnvalidatedArrivalDeadline
}

// ----------------------
// Input Command
// ----------------------
type Command<'data> = {
    Data: 'data
    Timestamp: DateTime
}

// I like the specific of `Book Cargo Form`
//type BookCargoCommand = {
//    BookCargoForm: UnvalidatedBookCargo
//    Timestamp: DateTime
//    UserId: string
//}

type BookCargoCommand = Command<UnvalidatedBookCargo>

// ----------------------
// Public API
// ----------------------

// Success output of Booking Cargo
type BookedCargo = {
    TrackingId: TrackingId
    Origin: ValidatedLocation
    Destination: ValidatedLocation
    ArrivalDeadline: ValidatedArrivalDeadline
}


type Cargo = {
    Id: TrackingId
    Origin: Location
    Destination: Location
    ArrivalDeadline: DateTime
}

// Location

type ValidatedLocation = private ValidatedLocation of Location
type LocationValidationService = UnvalidatedLocation -> ValidatedLocation option

// Arrival Deadline

type ValidatedArrivalDeadline = private ValidatedArrivalDeadline of DateTime
type ArrivalDeadlineValidationService = UnvalidatedArrivalDeadline -> ValidatedArrivalDeadline option



type ValidatedBookCargo = {
    TrackingId: TrackingId
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
type ValidateBookCargo = UnvalidatedLocation -> Result<ValidatedBookCargo, ValidationError>
type Booking = ValidatedBookCargo -> BookedCargo

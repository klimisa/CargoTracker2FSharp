namespace Domain.Shipping

open System

open Domain.Location
open Domain.Voyage

type TrackingId = TrackingId of Guid

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
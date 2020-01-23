namespace Domain.Shipping.Voyage

open System

type VoyageNumber = private VoyageNumber of string

module VoyageNumber =
    let value (VoyageNumber str) = str
    let create value =
        if String.IsNullOrWhiteSpace value then Error "VoyageNumber cannot be empty or null."
        else Ok (VoyageNumber value)
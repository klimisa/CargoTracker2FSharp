namespace Domain

open System.Collections.Generic

type IEvent = interface end

[<AllowNullLiteral>]
[<AbstractClass>]
type BaseAggregateRoot() =
    member this.Events = List<IEvent>() :> IList<IEvent>
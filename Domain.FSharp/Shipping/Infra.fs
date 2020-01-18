namespace Domain

open System.Collections.Generic

type IEvent = interface end

[<AllowNullLiteral>]
[<AbstractClass>]
type BaseAggregateRoot() =
    member val Events = List<IEvent>() :> IList<IEvent>
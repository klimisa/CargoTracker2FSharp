using Domain.Shipping.Cargo;
using Domain.Shipping.Location;
using Domain.Shipping.Voyage;

namespace App.DTO
{
    public static class Mapper
    {
        public static void Map()
        {
            AutoMapper.Mapper.Initialize(cfg => {

                cfg.CreateMap<Leg, Domain.Shipping.Cargo.Leg>()
                    .ConstructUsing( o => new Domain.Shipping.Cargo.Leg(
                        new VoyageNumber(o.Voyage)
                        , new UnLocode(o.LoadLocation)
                        , new UnLocode(o.UnloadLocation)
                        , o.LoadTime
                        , o.UnloadTime) );

                cfg.CreateMap<Itinerary, Domain.Shipping.Cargo.Itinerary>();

                cfg.CreateMap<RouteSpecification, Domain.Shipping.Cargo.RouteSpecification>()
                    .ConstructUsing( o => new Domain.Shipping.Cargo.RouteSpecification(
                        new UnLocode(o.Origin)
                        , new UnLocode(o.Destination)
                        , o.ArrivalDeadline) ) ;

                cfg.CreateMap<HandlingEvent, Domain.Shipping.Cargo.HandlingEvent>()
                    .ConstructUsing(o => new Domain.Shipping.Cargo.HandlingEvent(
                        new TrackingId(o.TrackingId)
                        , (HandlingType)o.Type
                        , new UnLocode(o.Location)
                        , new VoyageNumber(o.Voyage)
                        , o.Completed
                        , o.Registered
                        ));

            });
        }

    }

    public static class MappingExtensions
    {

        public static Domain.Shipping.Cargo.Leg ToDomain(this Leg leg)
        {
            return AutoMapper.Mapper.Map<Domain.Shipping.Cargo.Leg>(leg);
        }

        public static Domain.Shipping.Cargo.Itinerary ToDomain(this Itinerary itinerary)
        {
            return AutoMapper.Mapper.Map<Domain.Shipping.Cargo.Itinerary>(itinerary);
        }

        public static Domain.Shipping.Cargo.RouteSpecification ToDomain(this RouteSpecification routeSpec)
        {
            return AutoMapper.Mapper.Map<Domain.Shipping.Cargo.RouteSpecification>(routeSpec);
        }

        public static Domain.Shipping.Cargo.HandlingEvent ToDomain(this HandlingEvent handlingEvent)
        {
            return AutoMapper.Mapper.Map<Domain.Shipping.Cargo.HandlingEvent>(handlingEvent);
        }
    }
}

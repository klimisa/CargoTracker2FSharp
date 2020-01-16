using System;
using Domain.Shipping.Location;

namespace Domain.Shipping.Voyage
{
    public class CarrierMovement
    {
        public UnLocode DepartureLocation { get; }

        public UnLocode ArrivalLocation { get; }

        public DateTime DepartureTime { get; }

        public DateTime ArrivalTime { get; }

        public CarrierMovement(
            UnLocode departureLocation,
            UnLocode arrivalLocation,
            DateTime departureTime,
            DateTime arrivalTime)
        {
            // TODO: validation

            DepartureLocation = departureLocation;
            ArrivalLocation = arrivalLocation;
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
        }

    }
}

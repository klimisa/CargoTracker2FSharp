using System;
using Domain.Shipping.Location;
using Domain.Shipping.Voyage;

namespace Domain.Shipping.Cargo
{
    public class Leg
    {
        public VoyageNumber Voyage { get; }

        public UnLocode LoadLocation { get; }

        public UnLocode UnloadLocation { get; }

        public DateTime LoadTime { get; }

        public DateTime UnloadTime { get; }

        public Leg(VoyageNumber voyage,
            UnLocode loadLocation,
            UnLocode unloadLocation,
            DateTime loadTime,
            DateTime unloadTime)
        {
            Voyage = voyage ?? throw new ArgumentNullException(nameof(voyage));

            LoadLocation = loadLocation ?? throw new ArgumentNullException(nameof(loadLocation));

            UnloadLocation = unloadLocation ?? throw new ArgumentNullException(nameof(unloadLocation));

            if (loadTime >= unloadTime)
                throw new ArgumentException("unloadTime should be later than loadTime");

            LoadTime = loadTime;
            UnloadTime = unloadTime;
        }
    }
}
using System;
using Domain.Shipping.Location;
using Domain.Shipping.Voyage;

namespace Domain.Shipping.Cargo
{
    public class HandlingEvent
    {
        public TrackingId TrackingId { get; }

        public HandlingType Type { get; }

        public UnLocode Location { get; }

        public VoyageNumber Voyage { get; }

        public DateTime Completed { get; }

        public DateTime Registered { get; }

        public HandlingEvent(
            TrackingId trackingId
            , HandlingType type
            , UnLocode location
            , VoyageNumber voyage
            , DateTime completed
            , DateTime registered)
        {
            if ((type == HandlingType.Load || type == HandlingType.Unload) && voyage == null)
            {
                throw new InvalidOperationException("loading/unloading events need a voyage");
            }

            TrackingId = trackingId ?? throw new ArgumentNullException(nameof(trackingId));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Type = type;
            Voyage = voyage;
            Completed = completed;
            Registered = registered;
        }
    }
}
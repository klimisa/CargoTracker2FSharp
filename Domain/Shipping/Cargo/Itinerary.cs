using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Shipping.Location;
using Domain.Shipping.Voyage;

namespace Domain.Shipping.Cargo
{
    public class Itinerary 
    {
        public IReadOnlyCollection<Leg> Legs { get; }

        public Itinerary(IList<Leg> legs)
        {
            if (legs == null)
                throw new ArgumentNullException(nameof(legs));
            if (legs.Count == 0)
                throw new ArgumentException("legs cannot be empty");

            Legs = new ReadOnlyCollection<Leg>(legs);
        }

        public UnLocode FirstLoadLocation => Legs.First().LoadLocation;

        public VoyageNumber FirstVoyage => Legs.First().Voyage;

        public UnLocode LastUnloadLocation => Legs.Last().UnloadLocation;

        public DateTime FinalArrivalDate => Legs.Last().UnloadTime;

        public Leg NextOf(UnLocode location)
        {
            var next = (Leg)null;
            var currentFound = false;

            using (var en = Legs.GetEnumerator())
                while (en.MoveNext())
                {
                    if (currentFound)
                    {
                        next = en.Current;
                        break;
                    }
                    if (en.Current != null && (en.Current.LoadLocation.Equals(location) || en.Current.UnloadLocation.Equals(location)))
                        currentFound = true;
                }

            return next;
        }

        public Leg Of(UnLocode location)
        {
            return Legs
                .FirstOrDefault(l => l.UnloadLocation.Equals(location) || l.LoadLocation.Equals(location));
        }

        public bool IsExpected(HandlingEvent @event) {
            // TODO: Have a look on how to represent that as a valid state
            if (@event == null)
                return true;

            // receive at the first leg's load location
            switch (@event.Type) { 
                case HandlingType.Receive:
                    return FirstLoadLocation.Equals(@event.Location);
                case HandlingType.Load:
                    return Legs.Any(leg => leg.LoadLocation.Equals(@event.Location));
                case HandlingType.Unload:
                    return Legs.Any(leg => leg.UnloadLocation.Equals(@event.Location));
                case HandlingType.Claim:
                case HandlingType.Customs:
                    return LastUnloadLocation.Equals(@event.Location);
            }

            return false;
        }

    }
}

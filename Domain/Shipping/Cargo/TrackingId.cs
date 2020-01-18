using System;

namespace Domain.Shipping.Cargo
{
    public class TrackingId
    {
        public TrackingId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }
    }
}
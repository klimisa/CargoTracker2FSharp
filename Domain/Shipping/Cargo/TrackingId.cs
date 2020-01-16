using System;

namespace Domain.Shipping.Cargo
{
    public class TrackingId
    {
        public TrackingId(Guid value)
        {
            Value = value;
        }

        public TrackingId()
        {
            Value = Guid.NewGuid();
        }

        public Guid Value { get; }
    }
}
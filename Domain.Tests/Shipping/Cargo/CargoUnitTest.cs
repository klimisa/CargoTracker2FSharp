

namespace Domain.Tests.Shipping.Cargo
{
    using System;
    using AutoFixture.Xunit2;
    using Infra;
    using Xunit;
    using AutoFixture;
    using Domain.Shipping;

    public class CargoUnitTest
    {
        [Theory]
        [AutoData]
        public void Ctor__NoTrackingIdGiven__ThrowsArgumentNullException(
            RouteSpecification routeSpec
            )
        {
            Assert.Throws<ArgumentNullException>(() => new Cargo(null, routeSpec));
        }

        [Theory]
        [AutoData]
        public void Ctor__NoRouteSpecGiven__ThrowsArgumentNullException(
            TrackingId trackingId
        )
        {
            Assert.Throws<ArgumentNullException>(() => new Cargo(trackingId, null));
        }

        [Theory]
        [AutoData]
        public void Ctor__EmitsNewBookEvent(
            TrackingId trackingId,
            RouteSpecification routeSpec
        )
        {
            // ACT
            var sut = new Cargo(trackingId, routeSpec);

            // ASSERT
            Assert.Equal(routeSpec, sut.RouteSpec);
            Assert.Equal(routeSpec, sut.Delivery.RouteSpec);

            sut.Events[0].Should().BeEquivalentTo(new Events.NewBooked(trackingId, routeSpec));
        }   

        [Theory]
        [AutoData]
        public void AssignToItinerary__NoItineraryGiven__ThrowsArgumentNullException(
            Cargo sut
        )
        {
            Assert.Throws<ArgumentNullException>(() => sut.AssignToItinerary(null));
        }

        [Theory]
        [AutoData]
        public void AssignToItinerary__EmitsAssignedToItineraryEvent_and_EmitsDeliveryStateChanged(
            Cargo sut
        )
        {
            // ARRANGE
            var itinerary = new Fixture().Customize(new DefaultItineraryCustomization()).Create<Itinerary>();

            // ACT
            sut.AssignToItinerary(itinerary);

            // ASSERT
            Assert.Equal(itinerary, sut.Itinerary);
            Assert.Equal(itinerary, sut.Delivery.Itinerary);

            sut.Events[1].Should().BeEquivalentTo(new Events.AssignedToItinerary(sut.TrackingId, itinerary));
            sut.Events[2].Should().BeEquivalentTo(new Events.DeliveryStateChanged(sut.TrackingId, sut.Delivery));
        }

        [Theory]
        [AutoData]
        public void ChangeRoute__NoRouteSpecGiven__ThrowsArgumentNullException(
                Cargo sut
            )
        {
            Assert.Throws<ArgumentNullException>(() => sut.ChangeRoute(null));
        }

        [Theory]
        [AutoData]
        public void ChangeRoute__EmitsAssignedToItineraryEvent_and_EmitsDeliveryStateChangedEvent(
            Cargo sut,
            RouteSpecification routeSpec
        )
        {
            // ACT
            sut.ChangeRoute(routeSpec);

            // ASSERT
            Assert.Equal(routeSpec, sut.RouteSpec);
            Assert.Equal(routeSpec, sut.Delivery.RouteSpec);

            sut.Events[1].Should().BeEquivalentTo(new Events.RouteChanged(sut.TrackingId, routeSpec));
            sut.Events[2].Should().BeEquivalentTo(new Events.DeliveryStateChanged(sut.TrackingId, sut.Delivery));
        }

        [Theory]
        [AutoData]
        public void RegisterHandlingEvent__EmitsHandlingEventRegisteredEvent_and_EmitsDeliveryStateChangedEvent(
            Cargo sut,
            HandlingEvent @event
        )
        {
            // ACT
            sut.RegisterHandlingEvent(@event);

            // ASSERT
            Assert.Equal(@event, sut.LastHandlingEvent);
            Assert.Equal(@event, sut.Delivery.LastHandlingEvent);

            sut.Events[1].Should().BeEquivalentTo(new Events.HandlingEventRegistered(@event));
            sut.Events[2].Should().BeEquivalentTo(new Events.DeliveryStateChanged(sut.TrackingId, sut.Delivery));
        }

    }
}

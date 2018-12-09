using FluentAssertions;
using Xunit;

namespace EventFilter
{
    public class ShouldNotBlock
    {
        [Fact]
        public void WhenEventIsWhitelisted()
        {
            var events = new[] { "insert", "find" };
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(events);

            var isApproved = eventFilter.IsApproved("find");

            isApproved.Should().BeTrue();
        }

        [Fact]
        public void WhenWhitelistOfEventsIsEmpty()
        {
            string[] emptyListOfEvents = new string[0];
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(emptyListOfEvents);

            var isApproved = eventFilter.IsApproved("find");

            isApproved.Should().BeTrue();
        }

    }
}

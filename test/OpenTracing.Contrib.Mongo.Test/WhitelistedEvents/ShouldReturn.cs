using FluentAssertions;
using OpenTracing.Contrib.Mongo.Tracer;
using Xunit;

namespace WhitelistedEvents
{
    public class ShouldReturn
    {
        [Fact, Trait("Category", "Unit")]
        public void True_WhenEventIsWhitelisted()
        {
            var events = new [] {"insert", "find"};
            var eventFilter = new EventFilter(events);

            var isApproved = eventFilter.IsApproved("find");

            isApproved.Should().BeTrue();
        }

        [Fact, Trait("Category", "Unit")]
        public void False_WhenEventIsNotWhitelisted()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new EventFilter(events);

            var isApproved = eventFilter.IsApproved("some event");

            isApproved.Should().BeFalse();
        }

        [Fact, Trait("Category", "Unit")]
        public void False_WhenEventIsNull()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new EventFilter(events);

            var isApproved = eventFilter.IsApproved(null);

            isApproved.Should().BeFalse();
        }

        [Fact, Trait("Category", "Unit")]
        public void False_WhenEventIsEmptyString()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new EventFilter(events);

            var isApproved = eventFilter.IsApproved("");

            isApproved.Should().BeFalse();
        }
    }
}

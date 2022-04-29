using System;
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
            var maskedEvents = Array.Empty<string>();
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(events, maskedEvents);

            var isApproved = eventFilter.IsApproved("find");

            isApproved.Should().BeTrue();
        }

        [Fact]
        public void WhenWhitelistOfEventsIsEmpty()
        {
            string[] emptyListOfEvents = new string[0];
            var maskedEvents = Array.Empty<string>();
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(emptyListOfEvents, maskedEvents);

            var isApproved = eventFilter.IsApproved("find");

            isApproved.Should().BeTrue();
        }

    }
}

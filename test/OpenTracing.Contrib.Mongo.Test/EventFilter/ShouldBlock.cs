
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace EventFilter
{
    public class ShouldBlock
    {
        [Fact]
        public void WhenEventIsNotWhitelisted()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(events);

            var isApproved = eventFilter.IsApproved("some event");

            isApproved.Should().BeFalse();
        }

        [Fact]
        public void WhenEventIsNull()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(events);

            var isApproved = eventFilter.IsApproved(null);

            isApproved.Should().BeFalse();
        }

        [Fact]
        public void WhenEventIsEmptyString()
        {
            var events = new[] { "insert", "update" };
            var eventFilter = new OpenTracing.Contrib.Mongo.Tracer.EventFilter(events);

            var isApproved = eventFilter.IsApproved("");

            isApproved.Should().BeFalse();
        }
    }
}

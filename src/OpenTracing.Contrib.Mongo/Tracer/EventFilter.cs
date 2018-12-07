using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OpenTracing.Contrib.Mongo.Test")]
namespace OpenTracing.Contrib.Mongo.Tracer
{
    internal class EventFilter
    {
        private readonly string[] _events;

        public EventFilter(string[] events)
        {
            _events = events;
        }

        public bool IsApproved(string eventName)
        {
            var index = Array.IndexOf(_events, eventName);
            return index >= 0;
        }
    }
}

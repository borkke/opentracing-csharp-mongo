using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OpenTracing.Contrib.Mongo.Test")]
namespace OpenTracing.Contrib.Mongo.Tracer
{
    internal class EventFilter
    {
        private readonly string[] _allowedEvents;
        private readonly string[] _masskedEvents;

        public EventFilter(string[] allowedEvents, string[] masskedEvents)
        {
            _allowedEvents = allowedEvents;
            _masskedEvents = masskedEvents;
        }

        public bool IsApproved(string eventName)
        {
            if (_allowedEvents.Length == 0) return true;

            var index = Array.IndexOf(_allowedEvents, eventName);
            return index >= 0;
        }

        public bool IsMasked(string eventName)
        {
            if (_masskedEvents.Length == 0) return false;

            var index = Array.IndexOf(_masskedEvents, eventName);
            return index >= 0;
        }
    }
}

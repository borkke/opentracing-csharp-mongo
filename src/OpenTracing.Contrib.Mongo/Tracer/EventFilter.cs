using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OpenTracing.Contrib.Mongo.Test")]
namespace OpenTracing.Contrib.Mongo.Tracer
{
    internal class EventFilter
    {
        private readonly string[] _allowedEvents;
        private readonly string[] _masskedEvents;
        private readonly string[] _maskedFields;

        public EventFilter(string[] allowedEvents, string[] masskedEvents, string[] maskedFields)
        {
            _allowedEvents = allowedEvents;
            _masskedEvents = masskedEvents;
            _maskedFields = maskedFields;
        }

        public bool IsApproved(string eventName)
        {
            if (_allowedEvents.Length == 0) return true;

            var index = Array.IndexOf(_allowedEvents, eventName);
            return index >= 0;
        }

        public bool IsMasked(string eventName, string fieldName)
        {
            if (_masskedEvents.Length == 0) return false;
            if (_maskedFields.Length == 0) return true; //by default we mask

            var maskedEventsIndex = Array.IndexOf(_masskedEvents, eventName);
            var maskedFieldsIndex = Array.IndexOf(_maskedFields, fieldName);

            return maskedEventsIndex >= 0 && maskedFieldsIndex >= 0;
        }
    }
}

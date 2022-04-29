using System.Collections.Concurrent;
using System.Collections.Generic;
using MongoDB.Driver.Core.Events;
using OpenTracing.Contrib.Mongo.Configuration;
using OpenTracing.Tag;

namespace OpenTracing.Contrib.Mongo.Tracer
{
    internal class MongoEventListener
    {
        private const string MongoDbPrefix = "mongodb.";

        private readonly ITracer _tracer;
        private readonly EventFilter _eventFilter;
        private readonly ConcurrentDictionary<int, ISpan> _spanCache;

        public MongoEventListener(ITracer tracer, TracingOptions options)
        {
            _tracer = tracer;
            _eventFilter = new EventFilter(options.WhitelistedEvents, options.MasskedEvents);
            _spanCache = new ConcurrentDictionary<int, ISpan>();
        }

        public void StartEventHandler(CommandStartedEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            var span = BuildNewSpanWithDefaultTags(@event)
                .Start();

            _spanCache.TryAdd(@event.RequestId, span);
        }

        public void SuccessEventHandler(CommandSucceededEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            if (_spanCache.TryRemove(@event.RequestId, out var activeScope))
            {
                if(_eventFilter.IsMasked(@event.CommandName))
                {
                    activeScope.SetTag($"{MongoDbPrefix}reply", "*****");
                } else 
                {
                    activeScope.SetTag($"{MongoDbPrefix}reply", @event.Reply.ToString());
                }
                activeScope.Finish();
            }
        }

        public void ErrorEventHandler(CommandFailedEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            if (_spanCache.TryRemove(@event.RequestId, out var span))
            {
                span.Log(ExtractExceptionInfo(@event));
                span.SetTag(Tags.Error, true);
                span.Finish();
            }
        }

        private Dictionary<string, object> ExtractExceptionInfo(CommandFailedEvent @event)
        {
            return new Dictionary<string, object>
                {
                    { "event", "error" },
                    { "type", @event.Failure.GetType()},
                    { "message", @event.Failure.Message },
                    { "stack-trace", @event.Failure.StackTrace }
                };
        }

        private ISpanBuilder BuildNewSpanWithDefaultTags(CommandStartedEvent @event)
        {
            var tracer = _tracer
                .BuildSpan($"{MongoDbPrefix}{@event.CommandName}")
                .WithTag(Tags.SpanKind, Tags.SpanKindClient)
                .WithTag(Tags.Component, "csharp-mongo")
                .WithTag(Tags.DbInstance, @event.DatabaseNamespace.DatabaseName)
                .WithTag("db.host", @event.ConnectionId.ToString())
                .WithTag(Tags.DbType, "mongo");

            if(_eventFilter.IsMasked(@event.CommandName))
            {
                tracer = tracer.WithTag(Tags.DbStatement, "*****");
            } else 
            {
                tracer = tracer.WithTag(Tags.DbStatement, @event.Command.ToString());
            }

            return tracer;
        }
    }
}

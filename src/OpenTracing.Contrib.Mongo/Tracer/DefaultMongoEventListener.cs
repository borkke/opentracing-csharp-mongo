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
        private readonly ConcurrentDictionary<int, IScope> _scopeCache;

        public MongoEventListener(ITracer tracer, TracingOptions options)
        {
            _tracer = tracer;
            _eventFilter = new EventFilter(options.WhitelistedEvents);
            _scopeCache = new ConcurrentDictionary<int, IScope>();
        }

        public void StartEventHandler(CommandStartedEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            var scope = BuildNewScopeWithDefaultTags(@event)
                .StartActive(true);

            _scopeCache.TryAdd(@event.RequestId, scope);
        }

        public void SuccessEventHandler(CommandSucceededEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            if (_scopeCache.TryRemove(@event.RequestId, out var activeScope))
            {
                activeScope.Span.SetTag($"{MongoDbPrefix}reply", @event.Reply.ToString());
                activeScope.Dispose();
            }
        }

        public void ErrorEventHandler(CommandFailedEvent @event)
        {
            if (!_eventFilter.IsApproved(@event.CommandName))
                return;

            if (_scopeCache.TryRemove(@event.RequestId, out var activeScope))
            {
                activeScope.Span.Log(ExtractExceptionInfo(@event));
                activeScope.Dispose();
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

        private ISpanBuilder BuildNewScopeWithDefaultTags(CommandStartedEvent @event)
        {
            return _tracer
                .BuildSpan($"{MongoDbPrefix}{@event.CommandName}")
                .WithTag(Tags.SpanKind, Tags.SpanKindClient)
                .WithTag(Tags.Component, "csharp-mongo")
                .WithTag(Tags.DbStatement, @event.Command.ToString())
                .WithTag(Tags.DbInstance, @event.DatabaseNamespace.DatabaseName)
                .WithTag("db.host", @event.ConnectionId.ToString())
                .WithTag(Tags.DbType, "mongo");
        }
    }
}

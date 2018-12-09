using System;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using OpenTracing.Contrib.Mongo.Configuration;
using OpenTracing.Contrib.Mongo.Tracer;

namespace OpenTracing.Contrib.Mongo
{
    internal class MongoClientSettingsBuilder
    {
        private readonly ITracer _tracer;
        private MongoClientSettings _mongoClientSettings;
        private Action<TracingOptions> _options;

        private MongoClientSettingsBuilder(ITracer tracer)
        {
            _tracer = tracer;
        }

        internal static MongoClientSettingsBuilder WithTracer(ITracer tracer)
        {
            return new MongoClientSettingsBuilder(tracer);
        }

        internal MongoClientSettingsBuilder WithOptions(Action<TracingOptions> options)
        {
            _options = options;
            return this;
        }

        internal MongoClientSettingsBuilder WithMongoUri(MongoUrl mongoUrl)
        {
            _mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            return this;
        }

        internal MongoClientSettingsBuilder WithConnectionString(string connectionString)
        {
            _mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
            return this;
        }

        internal MongoClientSettingsBuilder WithMongoClientSettings(MongoClientSettings mongoClientSettings)
        {
            _mongoClientSettings = mongoClientSettings;
            return this;
        }

        internal MongoClientSettings Build()
        {
            if (_tracer == null) throw new ArgumentException("Tracer should be provided", nameof(_tracer));

            if (_mongoClientSettings == null) _mongoClientSettings = new MongoClientSettings();

            var tracingOptions = new TracingOptions();
            _options?.Invoke(tracingOptions);

            var mongoEventListener = new MongoEventListener(_tracer, tracingOptions);
            var clientsConfiguration = _mongoClientSettings.ClusterConfigurator;
            _mongoClientSettings.ClusterConfigurator = builder =>
            {
                builder
                    .Subscribe<CommandStartedEvent>(mongoEventListener.StartEventHandler)
                    .Subscribe<CommandSucceededEvent>(mongoEventListener.SuccessEventHandler)
                    .Subscribe<CommandFailedEvent>(mongoEventListener.ErrorEventHandler);
                clientsConfiguration?.Invoke(builder);
            };

            return _mongoClientSettings;
        }
    }
}

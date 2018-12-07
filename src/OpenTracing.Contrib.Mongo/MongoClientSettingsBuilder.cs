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
        private Action<TracongOptions> _options;

        private MongoClientSettingsBuilder(ITracer tracer)
        {
            _tracer = tracer;
        }

        internal static MongoClientSettingsBuilder WithTracer(ITracer tracer)
        {
            return new MongoClientSettingsBuilder(tracer);
        }

        internal MongoClientSettingsBuilder WithOptions(Action<TracongOptions> options)
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

            var tracingOptions = GetTracingOptions();
            var mongoEventListener = new MongoEventListener(_tracer, tracingOptions);
            SetMongoClientSettings(mongoEventListener);

            return _mongoClientSettings;
        }

        private TracongOptions GetTracingOptions()
        {
            var tracingOptions = new TracongOptions
            {
                WhitelistedEvents = DetaultEvents.Events
            };
            _options?.Invoke(tracingOptions);
            return tracingOptions;
        }

        public void SetMongoClientSettings(MongoEventListener mongoEventListener)
        {
            if (_mongoClientSettings.ClusterConfigurator == null)
            {
                _mongoClientSettings.ClusterConfigurator = builder => RegisterTracingHandlers(builder, mongoEventListener);
            }
            else
            {
                var clientsConfiguration = _mongoClientSettings.ClusterConfigurator;
                _mongoClientSettings.ClusterConfigurator = builder =>
                {
                    RegisterTracingHandlers(builder, mongoEventListener);
                    clientsConfiguration.Invoke(builder);
                };
            }
        }

        private static void RegisterTracingHandlers(ClusterBuilder builder, MongoEventListener mongoEventListener)
        {
            builder
                .Subscribe<CommandStartedEvent>(mongoEventListener.StartEventHandler)
                .Subscribe<CommandSucceededEvent>(mongoEventListener.SuccessEventHandler)
                .Subscribe<CommandFailedEvent>(mongoEventListener.ErrorEventHandler);
        }
    }
}

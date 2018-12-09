using System;
using MongoDB.Driver;
using OpenTracing.Contrib.Mongo.Configuration;
using OpenTracing.Util;

namespace OpenTracing.Contrib.Mongo
{
    public class TracingMongoClient : MongoClient
    {
        public TracingMongoClient(Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(GlobalTracer.Instance)
                .WithOptions(options)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .Build())
        {
        }

        public TracingMongoClient(MongoClientSettings mongoClientSettings, Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(GlobalTracer.Instance)
                .WithOptions(options)
                .WithMongoClientSettings(mongoClientSettings)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, MongoClientSettings mongoClientSettings, Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithMongoClientSettings(mongoClientSettings)
                .Build())
        {
        }

        public TracingMongoClient(MongoUrl mongoUrl, Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(GlobalTracer.Instance)
                .WithOptions(options)
                .WithMongoUri(mongoUrl)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, MongoUrl mongoUrl, Action<TracingOptions> options = null) 
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithMongoUri(mongoUrl)
                .Build())
        {
        }

        public TracingMongoClient(string connectionString, Action<TracingOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(GlobalTracer.Instance)
                .WithOptions(options)
                .WithConnectionString(connectionString)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, string connectionString, Action<TracingOptions> options = null) 
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithConnectionString(connectionString)
                .Build())
        {
        }
    }
}

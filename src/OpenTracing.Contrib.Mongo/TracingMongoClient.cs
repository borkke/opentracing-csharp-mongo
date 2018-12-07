using System;
using MongoDB.Driver;
using OpenTracing.Contrib.Mongo.Configuration;

namespace OpenTracing.Contrib.Mongo
{
    public class TracingMongoClient : MongoClient
    {
        public TracingMongoClient(ITracer tracer, Action<TracongOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, MongoClientSettings mongoClientSettings, Action<TracongOptions> options = null)
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithMongoClientSettings(mongoClientSettings)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, MongoUrl mongoUrl, Action<TracongOptions> options = null) 
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithMongoUri(mongoUrl)
                .Build())
        {
        }

        public TracingMongoClient(ITracer tracer, string connectionString, Action<TracongOptions> options = null) 
            : base(MongoClientSettingsBuilder.WithTracer(tracer)
                .WithOptions(options)
                .WithConnectionString(connectionString)
                .Build())
        {
        }
    }
}

using System.Linq;
using System.Runtime.InteropServices;
using FluentAssertions;
using MongoDB.Driver;
using OpenTracing;
using OpenTracing.Contrib.Mongo.Test;
using OpenTracing.Contrib.Mongo.Test.Model;
using OpenTracing.Mock;
using OpenTracing.Util;
using Xunit;

namespace TracingMongoClient
{
    [Collection("Database collection")]
    public class ShouldInitializeUsing
    {
        private readonly MongoFixture _fixture;

        public ShouldInitializeUsing(MongoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip = "Run this test only locally only if there is mongo running on default port")]
        public void DefaultConstructorAndTracerInstance()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "red"
            });

            tracer
                .FinishedSpans()
                .FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"))
                .Should().NotBeNull();
        }

        [Fact]
        public void MongoSettingsAndTracerInstance()
        {
            var tracer = new MockTracer();
            var clientSettings = MongoClientSettings.FromConnectionString(_fixture.TestMongoDb.ConnectionString);
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, clientSettings);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "red"
            });

            tracer
                .FinishedSpans()
                .FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"))
                .Should().NotBeNull();
        }

        [Fact]
        public void ConnectionStringAndTracerInstance()
        {
            var tracer = new MockTracer();
            var connectionString = _fixture.TestMongoDb.ConnectionString;
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, connectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "red"
            });

            tracer
                .FinishedSpans()
                .FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"))
                .Should().NotBeNull();
        }

        [Fact]
        public void MongoUrlAndTracerInstance()
        {
            var tracer = new MockTracer();
            var mongoUrl = new MongoUrl(_fixture.TestMongoDb.ConnectionString);
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, mongoUrl);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "red"
            });

            tracer
                .FinishedSpans()
                .FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"))
                .Should().NotBeNull();
        }
    }
}

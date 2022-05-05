using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using OpenTracing.Contrib.Mongo.Test;
using OpenTracing.Contrib.Mongo.Test.Model;
using OpenTracing.Contrib.Mongo.Test.TracingMongoClient.TestDoubles;
using OpenTracing.Mock;
using OpenTracing.Tag;
using Xunit;

namespace TracingMongoClient
{
    [Collection("Database collection")]
    public class ShouldCreate
    {
        private readonly MongoFixture _fixture;

        public ShouldCreate(MongoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ASpanWithDefaultFields()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnut = new Doughnut
            {
                Price = 12,
                Color = "red"
            };
            doughnutCollection.InsertOne(doughnut);

            var insertSpan = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName.StartsWith("mongodb."));
            insertSpan.Should().NotBeNull();
            insertSpan.OperationName.Should().StartWith("mongodb.");
            insertSpan.Tags.Count.Should().Be(7);
            insertSpan.Tags.Should().ContainKey(Tags.SpanKind.Key);
            insertSpan.Tags.Should().ContainKey(Tags.Component.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbStatement.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbInstance.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbType.Key);
            insertSpan.Tags.Should().ContainKey("mongodb.reply");
            insertSpan.Tags.Should().ContainKey("db.host");
        }

        [Fact]
        public void TwoDistinctSpans()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnutRed = new Doughnut
            {
                Price = 1,
                Color = "red"
            };
            doughnutCollection.InsertOne(doughnutRed);

            var doughnutGreen = new Doughnut
            {
                Price = 2,
                Color = "green"
            };
            doughnutCollection.InsertOne(doughnutGreen);

            var firstSpan = tracer.FinishedSpans().Last();
            var lastSpan = tracer.FinishedSpans().First();

            firstSpan.Should().NotBeNull();
            lastSpan.Should().NotBeNull();

            firstSpan.ParentId.Should().BeNullOrEmpty();
            lastSpan.ParentId.Should().BeNullOrEmpty();
        }

        [Fact]
        public void AChildSpan()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            using (var scope = tracer.BuildSpan("parentSpan").StartActive(true))
            {
                var doughnut = new Doughnut
                {
                    Price = 3,
                    Color = "gold"
                };
                doughnutCollection.InsertOne(doughnut);
            }

            var insertSpan = tracer.FinishedSpans().FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"));
            insertSpan.Should().NotBeNull();

            var parentSpan = tracer.FinishedSpans().FirstOrDefault(span => span.OperationName.Equals("parentSpan"));
            parentSpan.Should().NotBeNull();

            insertSpan.ParentId.Should().Be(parentSpan.Context.SpanId);
        }

        [Fact]
        public void ASpanAndCallOtherRegisteredEventHandlers()
        {
            var tracer = new MockTracer();
            var clientSettings = MongoClientSettings.FromConnectionString(_fixture.TestMongoDb.ConnectionString);
            var testHandler = new EventListenerTestDouble();
            clientSettings.ClusterConfigurator = builder =>
            {
                builder
                    .Subscribe<CommandStartedEvent>(@event =>
                    {
                        testHandler.StartEventHandler();
                    });
            };
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, clientSettings);
            var doughnutCollection = mongoClient.GetDatabase(_fixture.DATABASE_NAME).GetCollection<Doughnut>(_fixture.COLLECTION_NAME);

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "red"
            });

            var finishedSpan = tracer.FinishedSpans().FirstOrDefault(span => span.OperationName.Equals("mongodb.insert"));
            finishedSpan.Should().NotBeNull();
            testHandler.Counter.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public void ASpanForDefaultMongoEvent()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnut = new Doughnut
            {
                Price = 12,
                Color = "red"
            };
            doughnutCollection.InsertOne(doughnut);

            var insertSpan = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName == "mongodb.insert");
            insertSpan.Should().NotBeNull();
            insertSpan.OperationName.Should().Be("mongodb.insert");
        }

        [Fact]
        public void AnInformationSensitiveTagsByDefault()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString);
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnut = new Doughnut
            {
                Price = 12,
                Color = "P@ssword123"
            };
            doughnutCollection.InsertOne(doughnut);

            var span = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName == "mongodb.insert");
            span.Should().NotBeNull();
            var commandValue = span.Tags.GetValueOrDefault("db.statement").ToString();
            commandValue.Should().Contain("P@ssword123");
        }

        [Fact]
        public void AnInformationSensitiveTagsAndMaskTheirValueWhenOptionIsEnabled()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString, optiosns =>
            {
                optiosns.MaskedEvents = new string[] { "insert" };
            });
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnut = new Doughnut
            {
                Price = 12,
                Color = "P@ssword123"
            };
            doughnutCollection.InsertOne(doughnut);

            var span = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName == "mongodb.insert");
            span.Should().NotBeNull();
            var commandValue = span.Tags.GetValueOrDefault("db.statement").ToString();
            commandValue.Should().NotContain("P@ssword123");
            commandValue.Should().Contain("*****");
        }
    }
}

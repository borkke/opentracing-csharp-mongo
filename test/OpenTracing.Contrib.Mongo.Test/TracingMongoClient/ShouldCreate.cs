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

            tracer.FinishedSpans().Count.Should().Be(1);

            var insertSpan = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName.StartsWith("mongodb."));
            insertSpan.Should().NotBeNull();
            insertSpan.OperationName.Should().StartWith("mongodb.");
            insertSpan.Tags.Count.Should().Be(6);
            insertSpan.Tags.Should().ContainKey(Tags.SpanKind.Key);
            insertSpan.Tags.Should().ContainKey(Tags.Component.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbStatement.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbInstance.Key);
            insertSpan.Tags.Should().ContainKey(Tags.DbType.Key);
            insertSpan.Tags.Should().ContainKey("mongodb.reply");
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

            tracer.FinishedSpans().Count.Should().Be(2);
            tracer.FinishedSpans().Should().BeInAscendingOrder(a => a.StartTimestamp);

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

            using (var scope = tracer.BuildSpan("someWork").StartActive(true))
            {
                var doughnut = new Doughnut
                {
                    Price = 3,
                    Color = "gold"
                };
                doughnutCollection.InsertOne(doughnut);
            }

            tracer.FinishedSpans().Count.Should().Be(2);

            var firstSpan = tracer.FinishedSpans().Last();
            var lastSpan = tracer.FinishedSpans().First();

            firstSpan.Should().NotBeNull();
            lastSpan.Should().NotBeNull();

            firstSpan.ParentId.Should().BeNullOrEmpty();
            lastSpan.ParentId.Should().Be(firstSpan.Context.SpanId);
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

            tracer.FinishedSpans().Count.Should().Be(1);
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

            tracer.FinishedSpans().Count.Should().Be(1);

            var insertSpan = tracer.FinishedSpans().FirstOrDefault(sp => sp.OperationName == "mongodb.insert");
            insertSpan.Should().NotBeNull();
            insertSpan.OperationName.Should().Be("mongodb.insert");
        }
    }
}

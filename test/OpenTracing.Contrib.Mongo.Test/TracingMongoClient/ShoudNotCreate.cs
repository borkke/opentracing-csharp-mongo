using System.Linq;
using FluentAssertions;
using OpenTracing.Contrib.Mongo.Test;
using OpenTracing.Contrib.Mongo.Test.Model;
using OpenTracing.Mock;
using Xunit;

namespace TracingMongoClient
{
    [Collection("Database collection")]
    public class ShoudNotCreate
    {
        private readonly MongoFixture _fixture;

        public ShoudNotCreate(MongoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ASpanWhenMongoEventIsNotWhitelisted()
        {
            var tracer = new MockTracer();
            var mongoClient = new OpenTracing.Contrib.Mongo.TracingMongoClient(tracer, _fixture.TestMongoDb.ConnectionString, options =>
            {
                options.WhitelistedEvents = new[] {"update", "delete"};
            });
            var doughnutCollection = _fixture.GetDoughnutCollection(mongoClient);

            var doughnut = new Doughnut
            {
                Price = 12,
                Color = "red"
            };
            doughnutCollection.InsertOne(doughnut);

            tracer.FinishedSpans().Should().BeEmpty();
        }
    }
}

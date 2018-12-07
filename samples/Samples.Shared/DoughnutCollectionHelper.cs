using MongoDB.Driver;
using OpenTracing.Contrib.Mongo;
using OpenTracing.Util;

namespace Samples.Shared
{
    public static class DoughnutCollectionHelper
    {
        public static IMongoCollection<Doughnut> GetMongoClient()
        {
            var mongoClient = new MongoClient();
            return mongoClient.GetDatabase("DoughnutShop").GetCollection<Doughnut>("Doughnut");
        }

        public static IMongoCollection<Doughnut> GetTracingMongoClient()
        {
            var mongoClient = new TracingMongoClient(GlobalTracer.Instance);
            return mongoClient.GetDatabase("DoughnutShop").GetCollection<Doughnut>("Doughnut");
        }
    }
}

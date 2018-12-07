using System;
using Mongo2Go;
using MongoDB.Driver;
using OpenTracing.Contrib.Mongo.Test.Model;

namespace OpenTracing.Contrib.Mongo.Test
{
    public class MongoFixture : IDisposable
    {
        public string DATABASE_NAME = "DoughnutShopDatabase";
        public string COLLECTION_NAME = "Doughnut";

        public MongoFixture()
        {
            TestMongoDb = MongoDbRunner.Start();
        }

        public readonly MongoDbRunner TestMongoDb;

        public IMongoCollection<Doughnut> GetDoughnutCollection(IMongoClient mongoClient)
        {
            return mongoClient.GetDatabase(DATABASE_NAME).GetCollection<Doughnut>(COLLECTION_NAME);
        }

        public void Dispose()
        {
            TestMongoDb.Dispose();
        }
    }
}

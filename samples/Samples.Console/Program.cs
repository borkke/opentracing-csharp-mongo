using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenTracing.Util;
using Samples.Shared;

namespace Samples.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = new LoggerFactory().AddConsole();
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Starting app");

            var tracer = JaegerTracer.CreateTracer("console-app", loggerFactory);
            GlobalTracer.Register(tracer);

            var doughnutCollection = DoughnutCollectionHelper.GetTracingMongoClient();

            doughnutCollection.InsertOne(new Doughnut
            {
                Price = 1,
                Color = "Red"
            });

            var doughnuts = doughnutCollection.Find(a => true).ToList();

            doughnutCollection.FindOneAndDelete(a => a.Id == ObjectId.GenerateNewId());

            System.Console.ReadKey();
        }
    }
}

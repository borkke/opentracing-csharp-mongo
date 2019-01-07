using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenTracing;
using OpenTracing.Tag;
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

            using (var scope = tracer.BuildSpan("custom-span").StartActive(finishSpanOnDispose: true))
            {
                try
                {
                    var doughnuts = doughnutCollection.Find(a => true).ToList();
                    doughnutCollection.FindOneAndDelete(a => a.Id == ObjectId.GenerateNewId());
                    throw new Exception("Some exception");
                }
                catch (Exception e)
                {
                    scope.Span.SetTag(Tags.Error, true);
                    scope.Span.Log(new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("message", e.Message),
                        new KeyValuePair<string, object>("stack.trace", e.StackTrace),
                        new KeyValuePair<string, object>("source", e.Source)
                    });
                }
            }

            System.Console.ReadKey();
        }
    }
}

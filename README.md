# OpenTracing instrumentation for Mongodb driver

This repository contains instrumentation for c# mongodb driver. It is doing instrumentation by hooking into mongo driver's [database command](https://docs.mongodb.com/manual/reference/command/#database-operations) events.

[![Build status](https://ci.appveyor.com/api/projects/status/d0a3cdtik6b8pe6s/branch/master?svg=true)](https://ci.appveyor.com/project/borkke/opentracing-csharp-mongo-6iejj/branch/master) [![OpenTracing Badge](https://img.shields.io/badge/OpenTracing-enabled-blue.svg)](http://opentracing.io)

## Installation 
```
Install-Package OpenTracing.Contrib.Mongo
or
dotnet add package OpenTracing.Contrib.Mongo
```

## Usage

As a prerequisite, you need to have `ITracer` instance. Here you can find an example how to create [Jaeger tracer instance](https://github.com/borkke/opentracing-csharp-mongo/blob/develop/samples/Samples.Shared/JaegerTracer.cs). This is typically done once on application start.

```c#
ITracer tracer = ...

//If you are using using DI framework you can register tracer instance as singleton
//Also you can register with `GlobalTracer`
GlobalTracer.Register(tracer);
```
Once you have registered instance of a `ITracer` you can instantiate your mongo client with enabled instrumentation.

```c#
//Using default constructor
IMongoClient mongoClient = new TracingMongoClient();
IMongoClient mongoClient = new TracingMongoClient(GlobalTracer.Instance);

//By passing in connection string
IMongoClient mongoClient = new TracingMongoClient("<connection string>");
IMongoClient mongoClient = new TracingMongoClient(GlobalTracer.Instance, "<connection string>");

//By passing MongoUrl instance
IMongoClient mongoClient = new TracingMongoClient(new MongoUrl("..."));
IMongoClient mongoClient = new TracingMongoClient(GlobalTracer.Instance, new MongoUrl("..."));

//By passing `MongoClientSettings`
MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString("<connection string>");
IMongoClient mongoClient = new TracingMongoClient(clientSettings);
IMongoClient mongoClient = new TracingMongoClient(GlobalTracer.Instance, clientSettings);
```

### Customization
The `TracingMongoClient` allows you to controll what events you want to trace and it allows you to mask (replaced with `******`) some events and fields that might contain sensitive information.

These are [events](https://github.com/borkke/opentracing-csharp-mongo/blob/master/src/OpenTracing.Contrib.Mongo/Configuration/DetaultEvents.cs) that are instrumented by default. There are two properties that you can mask `mongodb.reply` and `db.statement`.
```c#
var mongoClient = new TracingMongoClient("<connection string>", options =>
{
    # Only "insert" and "find" events are instrumented
    options.WhitelistedEvents = new[] {"insert", "find"};
    # Masking
    # Only "insert" event will be masked
    optiosns.MaskedEvents = new string[] { "insert" };
    # For "insert" event, only "mongodb.reply" field will be masked
    optiosns.MaskedFields = new string[] { "mongodb.reply" };
});
```

## Samples
There are two samples in the `/samples` folder. One is the simple console application and one is the REST API which is already instrumented with [`OpenTracing.Contrib.NetCore`](https://github.com/opentracing-contrib/csharp-netcore) instrumentation.

In order to run samples, you will need mongo database on default port and [jaeger](https://www.jaegertracing.io/) tracer. You can run jaeger with this docker command:

```docker
docker run -d --name jaeger \
  -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 \
  -p 5775:5775/udp \
  -p 6831:6831/udp \
  -p 6832:6832/udp \
  -p 5778:5778 \
  -p 16686:16686 \
  -p 14268:14268 \
  -p 9411:9411 \
  jaegertracing/all-in-one:1.7
```

## Contributing

### Build 
```bash
dotnet build .
```

### Test
```bash
dotnet test .
```


using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenTracing.Contrib.Mongo.Test
{
    [CollectionDefinition("Database collection")]
    public class MongoCollection : ICollectionFixture<MongoFixture>
    {
    }
}

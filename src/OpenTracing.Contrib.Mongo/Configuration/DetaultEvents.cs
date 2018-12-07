namespace OpenTracing.Contrib.Mongo.Configuration
{
    internal static class DetaultEvents
    {
        //https://docs.mongodb.com/manual/reference/command/#database-operations
        internal static readonly string[] Events = {
            //aggregation
            "aggregate",
            "count",
            "distinct",
            "group",
            "mapReduce",
            //query and write
            "delete",
            "eval",
            "find",
            "findAndModify",
            "getMore",
            "insert",
            "update"
        };
    }
}

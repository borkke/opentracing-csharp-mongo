using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Samples.Shared
{
    public class Doughnut
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("color")]
        public string Color { get; set; }
        public int Price { get; set; }
        public long OwnerId { get; set; }
    }
}

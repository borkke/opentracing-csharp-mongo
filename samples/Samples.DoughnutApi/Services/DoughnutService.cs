using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Samples.Shared;

namespace Samples.DoughnutApi.Services
{
    public class DoughnutService
    {
        private readonly IMongoCollection<Doughnut> _doughnutCollection;

        public DoughnutService()
        {
            _doughnutCollection = DoughnutCollectionHelper.GetTracingMongoClient();
        }

        public List<Doughnut> Get()
        {
            return _doughnutCollection.Find(a => true).ToList();
        }

        public Doughnut GetById(string id)
        {
            return _doughnutCollection.Find(a => a.Id == new ObjectId(id)).FirstOrDefault();
        }

        public void Create(Doughnut doughnut)
        {
            _doughnutCollection.InsertOne(doughnut);
        }

        public void Update(Doughnut doughnut, string id)
        {
            _doughnutCollection.UpdateOne(
                Builders<Doughnut>.Filter.Eq("_id", new ObjectId(id)),
                Builders<Doughnut>.Update
                    .Set(a => a.Color, doughnut.Color)
                    .Set(a => a.Price, doughnut.Price)
                    .Set(a => a.OwnerId, doughnut.OwnerId));
        }

        public void Delete(string id)
        {
            _doughnutCollection.DeleteOne(a => a.Id == new ObjectId(id));
        }
    }
}

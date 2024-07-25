using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.ViewModels.MongoDb
{
    public class CartItemMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long client_id { get; set; }
        public int product_id { get; set; }
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.ViewModels.MongoDb
{
    public class BookingMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long client_id { get; set; }

        public List<CartItemMongoDbModel> products { get; set; }
    }
}

﻿using Entities.ViewModels.Products;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.MongoDb
{
    public class CartItemMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long account_client_id { get; set; }
        public int quanity { get; set; }
        public double? total_price { get; set; }
        public double? total_profit { get; set; }
        public double? total_discount { get; set; }
        public double total_amount { get; set; }
        public DateTime created_date { get; set; }
        public ProductMongoDbModel product { get; set; }
    }
}

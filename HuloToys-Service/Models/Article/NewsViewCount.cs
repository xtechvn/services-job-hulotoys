﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ENTITIES.ViewModels.ArticleViewModels
{
    public class NewsViewCount
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }
        public long articleID { get; set; }
        public long pageview { get; set; }   
    }
}

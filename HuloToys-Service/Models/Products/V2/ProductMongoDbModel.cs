﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Products.V2
{
    public class ProductMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }
        public string code { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }
        public int quanity_of_stock { get; set; }
        public List<string> images { get; set; }
        public string avatar { get; set; }
        public List<string> videos { get; set; }
        public string name { get; set; }
        public int group_product_id { get; set; }
        public string description { get; set; }
        public List<ProductAttributeMongoDbModel> specification { get; set; }
        public List<ProductAttributeMongoDbModel> attributes { get; set; }
        public List<ProductDiscountOnGroupsBuyModel> discount_group_buy { get; set; }
        public int preorder_status { get; set; }
        public int condition_of_product { get; set; }
        public string sku { get; set; }
        public string parent_product_id { get; set; }


    }
}

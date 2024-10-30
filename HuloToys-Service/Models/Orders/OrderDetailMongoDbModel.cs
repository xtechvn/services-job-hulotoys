﻿using HuloToys_Service.Models.NinjaVan;
using Models.APIRequest;
using Models.MongoDb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HuloToys_Service.Models.Orders
{
    public class OrderDetailMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long account_client_id { get; set; }
        public int payment_type { get; set; }
        public int delivery_type { get; set; }
        public long order_id { get; set; }
        public string order_no { get; set; }

        public double total_amount { get; set; }
        public double? total_price { get; set; }
        public double? total_profit { get; set; }
        public double? total_discount { get; set; }
        public List<CartItemMongoDbModel> carts { get; set; }
        public string utm_source { get; set; }
        public string utm_medium { get; set; }
        public int voucher_id { get; set; }

        public string receivername { get; set; }

        public string phone { get; set; }

        public string? provinceid { get; set; }

        public string? districtid { get; set; }

        public string? wardid { get; set; }

        public string address { get; set; }
        public long address_id { get; set; }
        public double? shipping_fee { get; set; } = 0;
        public ShippingFeeRequestModel delivery_detail { get; set; }

    }
}

using Nest;
using System;

public class ProductESModel
{
    [PropertyName("id")]
    public long id { get; set; } // ID ElasticSearch
    [PropertyName("product_id")]

    public string product_id { get; set; }
    [PropertyName("name")]

    public string name { get; set; }
    [PropertyName("amount")]

    public double amount { get; set; }
    [PropertyName("product_code")]

    public string product_code { get; set; }
    [PropertyName("description")]
    public string description { get; set; }
    [PropertyName("product_name_no_tv")]
    public string no_space_name { get; set; } // <-- TH�M N�Y
}
﻿using Entities.ViewModels.Products;

namespace HuloToys_Front_End.Models.Products
{
    public class ProductListResponseModel
    {
        public List<ProductMongoDbModel> items { get; set; }
        public long count { get; set; }
    }
}

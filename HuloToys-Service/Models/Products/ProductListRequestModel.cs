﻿namespace HuloToys_Front_End.Models.Products
{
    public class ProductListRequestModel
    {
        public string keyword { get; set; }
        public int group_id { get; set; }
        public int page_index { get; set; }
        public int page_size { get; set; }
        public double? price_from { get; set; }  // Giá bắt đầu
        public double? price_to { get; set; }    // Giá kết thúc
        public float? rating { get; set; }           // Sắp xếp
    }
}

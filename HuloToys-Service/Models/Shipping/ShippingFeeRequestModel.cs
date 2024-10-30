namespace HuloToys_Service.Models.NinjaVan
{
    public class ShippingFeeRequestModel
    {
        public int from_province_id { get; set; }
        public int to_province_id { get; set; }
        public int shipping_type { get; set; }
        public int carrier_id { get; set; }
        public string cart_id { get; set; }
        public string product_id { get; set; }
        public int quanity { get; set; }
    }
}

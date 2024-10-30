namespace HuloToys_Service.Models.NinjaVan
{
    public class ShippingFeeResponseModel
    {
        public int from_province_id { get; set; }
        public int to_province_id { get; set; }
        public int type { get; set; }
        public string cart_id { get; set; }
        public string product_id { get; set; }
        public int quanity { get; set; }
        public int shipping_fee { get; set; }
    }
   
}

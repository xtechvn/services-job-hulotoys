using Entities.ViewModels.Products;

namespace HuloToys_Front_End.Models.Products
{
    public class ProductAddToCartRequestModel
    {
        public ProductMongoDbModel product { get; set; }
        public long account_client_id { get; set; }
        public int quanity { get; set; }
    }
}

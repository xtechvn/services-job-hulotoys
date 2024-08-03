using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Products;

namespace HuloToys_Service.Models.Products
{
    public class ProductByGroupViewModel: SearchEsEntitiesViewModel
    {
        public List<LocationProduct> locationProducts { get; set; } // ds sp tìm được
    }
}

using Azure.Core;
using Caching.Elasticsearch;
using HuloToys_Front_End.Models.Products;
using HuloToys_Service.ElasticSearch;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.MongoDb;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Product.Bussiness
{
    public class ProductDetailService
    {
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        private readonly CartMongodbService _cartMongodbService;
        private readonly RaitingESService _raitingESService;
        private readonly ClientESService _clientESService;
        private readonly IConfiguration _configuration;
        private readonly GroupProductESService groupProductESService;

        public ProductDetailService(IConfiguration configuration)
        {
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            _cartMongodbService = new CartMongodbService(configuration);
            groupProductESService = new GroupProductESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            _raitingESService = new RaitingESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            _clientESService = new ClientESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            _configuration = configuration;
        }
        public async Task<ProductListFEResponseModel> ProductListing(ProductListRequestModel request)
        {
            ProductListFEResponseModel result = new ProductListFEResponseModel();
            try
            {
                var data = await _productDetailMongoAccess.ResponseListing(request.keyword, request.group_id,request.page_index,request.page_size);
                result = JsonConvert.DeserializeObject<ProductListFEResponseModel>(JsonConvert.SerializeObject(data));
                if(result!=null && result.items!=null && result.items.Count > 0)
                {
                    foreach (var i in result.items)
                    {
                        var raiting = _raitingESService.GetListByFilter(new Models.Raiting.ProductRaitingRequestModel()
                        {
                            id = i._id,
                            has_comment = false,
                            has_media = false,
                            page_index = 1,
                            page_size = 500,
                            stars = 0
                        });
                        i.review_count = raiting.Count;
                        i.rating = raiting.Sum(x=> x.star==null?0:(float)x.star) / (float)raiting.Count;
                    }
                }
            }
            catch
            {

            }
            return result;
        }
    }
}

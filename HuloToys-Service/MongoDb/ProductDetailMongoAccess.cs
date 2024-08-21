using Entities.ViewModels.Products;
using HuloToys_Front_End.Models.Products;
using HuloToys_Service.Models.Products;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace WEB.CMS.Models.Product
{
    public class ProductDetailMongoAccess
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<ProductMongoDbModel> _productDetailCollection;

        public ProductDetailMongoAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            string url = "mongodb://" + configuration["DataBaseConfig:MongoServer:Host"] + "";
            var client = new MongoClient("mongodb://" + configuration["DataBaseConfig:MongoServer:Host"] + "");
            IMongoDatabase db = client.GetDatabase(configuration["DataBaseConfig:MongoServer:catalog_core"]);
            _productDetailCollection = db.GetCollection<ProductMongoDbModel>("ProductDetail");
        }
        public async Task<string> AddNewAsync(ProductMongoDbModel model)
        {
            try
            {
                await _productDetailCollection.InsertOneAsync(model);
                return model._id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string> UpdateAsync(ProductMongoDbModel model)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.And(
                    filter.Eq("_id", model._id));
                await _productDetailCollection.FindOneAndReplaceAsync(filterDefinition, model);
                return model._id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<string> DeleteAsync(string id)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.And(
                    filter.Eq("_id", id)
                    );
                await _productDetailCollection.FindOneAndDeleteAsync(filterDefinition);
                return id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
      
        public async Task<ProductMongoDbModel> GetByID(string id)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<ProductMongoDbModel>.Filter.Eq(x => x._id, id); ;
                var model = await _productDetailCollection.Find(filterDefinition).FirstOrDefaultAsync();
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
     
        public async Task<ProductListResponseModel> Listing(string keyword = "", int group_id = -1, int page_index = 1, int page_size = 10)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                if(keyword!=null && keyword.Trim() != "")
                {
                    filterDefinition &= Builders<ProductMongoDbModel>.Filter.Regex(x => x.name, keyword);

                }
                if (group_id > 0)
                {
                    filterDefinition &= Builders<ProductMongoDbModel>.Filter.Regex(x => x.group_product_id, group_id.ToString());
                }
                var sort_filter = Builders<ProductMongoDbModel>.Sort;
                var sort_filter_definition = sort_filter.Descending(x=>x.updated_last);
                var model =  _productDetailCollection.Find(filterDefinition);
                long count = await model.CountDocumentsAsync();
                model.Options.Skip = page_index < 1 ? 0 : (page_index - 1) * page_size;
                model.Options.Limit = page_size;

                return new ProductListResponseModel()
                {
                    items= await model.ToListAsync(),
                    count=count
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       

    }
}

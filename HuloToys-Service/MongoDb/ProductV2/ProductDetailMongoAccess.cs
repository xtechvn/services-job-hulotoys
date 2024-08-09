using Entities.ViewModels;
using Entities.ViewModels.Product;
using Entities.ViewModels.Products;
using Entities.ViewModels.Products.V2;
using HuloToys_Service.Utilities.Lib;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HuloToys_Service.Models.Product.V2
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
            IMongoDatabase db = client.GetDatabase(configuration["DataBaseConfig:MongoServer:catalog"]);
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
        public async Task<string> DeleteAsync(ProductBlackList model)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.And(
                    filter.Eq("_id", model._id)
                    );
                await _productDetailCollection.FindOneAndDeleteAsync(filterDefinition);
                return model._id;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<string> FindIDByProductCode(string product_code)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<ProductMongoDbModel>.Filter.Eq(x => x.code, product_code); ;
                var model = await _productDetailCollection.Find(filterDefinition).FirstOrDefaultAsync();
                if(model!=null && model._id!=null && model._id.Trim()!="")
                return model._id;
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        public async Task<ProductMongoDbModel> FindByID(string id)
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
        public async Task<ProductMongoDbModel> FindDetailByProductCode(string product_code)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<ProductMongoDbModel>.Filter.Eq(x => x.code, product_code); ;
                var model = await _productDetailCollection.Find(filterDefinition).FirstOrDefaultAsync();
                return model;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
      
      
     
    }
}

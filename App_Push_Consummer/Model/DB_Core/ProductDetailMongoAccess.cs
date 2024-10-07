using Entities.ViewModels.Products;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WEB.CMS.Models.Product
{
    public class ProductDetailMongoAccess
    {
        private IMongoCollection<ProductMongoDbModel> _productDetailCollection;

        public ProductDetailMongoAccess()
        {
            var client = new MongoClient("mongodb://" + ConfigurationManager.AppSettings["Mongo_Host"] + "");
            IMongoDatabase db = client.GetDatabase(ConfigurationManager.AppSettings["Mongo_catalog"]);
            _productDetailCollection = db.GetCollection<ProductMongoDbModel>("ProductDetail");
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
        public async Task<string> UpdateStar(string id, float star)
        {
            try
            {
                var filter = Builders<ProductMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<ProductMongoDbModel>.Filter.Eq(x => x._id, id);
                var update = Builders<ProductMongoDbModel>.Update.Set(x => x.star, star);

                var updated_item = await _productDetailCollection.UpdateManyAsync(filterDefinition, update);
                return id;
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
            }
            return null;

        }
    }
}

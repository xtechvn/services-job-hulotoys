using Models.MongoDb;
using HuloToys_Service.Utilities.Lib;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;

namespace HuloToys_Service.MongoDb
{
    public class CartMongodbService
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<CartItemMongoDbModel> bookingCollection;

        public CartMongodbService(IConfiguration configuration) {

            _configuration= configuration;
            //      "connection_string": "mongodb://adavigolog_writer:adavigolog_2022@103.163.216.42:27017/?authSource=Adavigo"
            string _connection = "mongodb://" + _configuration["DataBaseConfig:MongoServer:user"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:pwd"]
                 + "@" + _configuration["DataBaseConfig:MongoServer:Host"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:Port"]
                 + "/?authSource=" + _configuration["DataBaseConfig:MongoServer:catalog_log"];
            var booking = new MongoClient(_connection);
            IMongoDatabase db = booking.GetDatabase(_configuration["DataBaseConfig:MongoServer:catalog_log"]);
            bookingCollection = db.GetCollection<CartItemMongoDbModel>("Cart");
        }
        public async Task<string> InsertCartItem(CartItemMongoDbModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }
        public async Task<int> CountCartItemByClientId(long client_id)
        {
            try
            {

                var filter = Builders<CartItemMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<CartItemMongoDbModel>.Filter.Eq(x => x.client_id, client_id);

                var model = await bookingCollection.Find(filterDefinition).ToListAsync();
                if (model != null)
                {
                    return model.Count;
                }
                   
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return 0;
        }
        public async Task<List<CartItemMongoDbModel>> GetCartItemByClientID(long client_id)
        {
            try
            {

                var filter = Builders<CartItemMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<CartItemMongoDbModel>.Filter.Eq(x => x.client_id, client_id);

                var model = await bookingCollection.Find(filterDefinition).ToListAsync();
                if (model != null)
                {
                    return model;
                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public async Task<bool> DeleteCartItemByID(string id)
        {
            try
            {

                var filter = Builders<CartItemMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<CartItemMongoDbModel>.Filter.Eq(x => x._id, id);

                var model = await bookingCollection.DeleteOneAsync(filterDefinition);
                return model.DeletedCount > 0;

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return false;
        }
    }
}

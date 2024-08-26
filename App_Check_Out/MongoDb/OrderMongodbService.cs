﻿using MongoDB.Driver;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using APP_CHECKOUT.Models.Orders;

namespace APP_CHECKOUT.MongoDb
{
    public class OrderMongodbService
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<OrderDetailMongoDbModel> bookingCollection;

        public OrderMongodbService(IConfiguration configuration) {

            _configuration= configuration;
            //      "connection_string": "mongodb://adavigolog_writer:adavigolog_2022@103.163.216.42:27017/?authSource=Adavigo"
            string _connection = "mongodb://" + _configuration["MongoServer:user"]
                 + ":" + _configuration["MongoServer:pwd"]
                 + "@" + _configuration["MongoServer:Host"]
                 + ":" + _configuration["MongoServer:Port"]
                 + "/?authSource=" + _configuration["MongoServer:catalog"];
            var booking = new MongoClient(_connection);
            IMongoDatabase db = booking.GetDatabase(_configuration["MongoServer:catalog"]);
            bookingCollection = db.GetCollection<OrderDetailMongoDbModel>("Orders");
        }
        public async Task<string> Insert(OrderDetailMongoDbModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                //string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                //LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }
      
        public async Task<OrderDetailMongoDbModel> FindById(string id)
        {
            try
            {
                var filter = Builders<OrderDetailMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<OrderDetailMongoDbModel>.Filter.Eq(x => x._id, id);

                var model = await bookingCollection.Find(filterDefinition).ToListAsync();
                if (model != null)
                {
                    return model.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                //LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }
       
        public async Task<bool> Delete(string id)
        {
            try
            {

                var filter = Builders<OrderDetailMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<OrderDetailMongoDbModel>.Filter.Eq(x => x._id, id);

                var model = await bookingCollection.DeleteOneAsync(filterDefinition);
                return model.DeletedCount > 0;

            }
            catch (Exception ex)
            {
                //string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                //LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return false;
        }
    }
}

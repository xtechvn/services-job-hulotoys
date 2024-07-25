using Entities.ViewModels.MongoDb;
using HuloToys_Service.Utilities.Lib;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;

namespace HuloToys_Service.MongoDb
{
    public class BookingMongodbService
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<BookingMongoDbModel> bookingCollection;

        public BookingMongodbService(IConfiguration configuration) {

            _configuration= configuration;
            //      "connection_string": "mongodb://adavigolog_writer:adavigolog_2022@103.163.216.42:27017/?authSource=Adavigo"
            string _connection = "mongodb://" + _configuration["DataBaseConfig:MongoServer:user"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:pwd"]
                 + "@" + _configuration["DataBaseConfig:MongoServer:Host"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:Port"]
                 + "/?authSource=" + _configuration["DataBaseConfig:MongoServer:catalog_log"];
            var booking = new MongoClient(_connection);
            IMongoDatabase db = booking.GetDatabase(_configuration["DataBaseConfig:MongoServer:catalog_log"]);
            bookingCollection = db.GetCollection<BookingMongoDbModel>("Booking");
        }
        public async Task<string> Insert(BookingMongoDbModel item)
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
    }
}

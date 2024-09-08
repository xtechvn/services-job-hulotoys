using HuloToys_Service.Models.Location;
using Models.MongoDb;

namespace HuloToys_Service.Models.Orders
{
    public class OrderDetailResponseModel
    {
        public OrderDetailMongoDbModel data_order { get; set; }
        public OrderESModel data { get; set; }
        public Province province { get; set; }
        public District district { get; set; }
        public Ward ward { get; set; }
    }
}

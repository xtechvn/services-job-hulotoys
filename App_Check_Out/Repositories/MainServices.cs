using APP.READ_MESSAGES.Libraries;
using APP_CHECKOUT.DAL;
using APP_CHECKOUT.Helpers;
using APP_CHECKOUT.Interfaces;
using APP_CHECKOUT.MongoDb;
using Entities.Models;
using APP_CHECKOUT.Models.Models.Queue;
using APP_CHECKOUT.Utilities.constants;
using Microsoft.Extensions.Configuration;
using APP_CHECKOUT.Elasticsearch;
using Newtonsoft.Json;
using HuloToys_Service.Models.Location;
using Nest;
using Caching.RedisWorker;
using Utilities.Contants;

namespace APP_CHECKOUT.Repositories
{
    public class MainServices: IMainServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingService logging_service;
        private readonly OrderMongodbService orderDetailMongoDbModel;
        private readonly OrderDAL orderDAL;
        private readonly OrderDetailDAL orderDetailDAL;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly RedisConn _redisService;
        public MainServices(IConfiguration configuration, ILoggingService loggingService) {

            _configuration=configuration;
            logging_service=loggingService;
            orderDetailMongoDbModel = new OrderMongodbService(configuration);
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            orderDAL = new OrderDAL(configuration["ConnectionString"]);
            orderDetailDAL = new OrderDetailDAL(configuration["ConnectionString"]);
            accountClientESService = new AccountClientESService(configuration["Elastic:Host"], configuration);
            clientESService = new ClientESService(configuration["Elastic:Host"], configuration);
        }
        public async Task Excute(CheckoutQueueModel request)
        {
            try
            {
                if (request == null || request.event_id<0) {
                    return;
                }
                switch (request.event_id)
                {
                    case (int)CheckoutEventID.CREATE_ORDER:
                        {
                            await CreateOrder(request.order_mongo_id);
                        }break;
                    case (int)CheckoutEventID.UPDATE_ORDER:
                        {

                        }
                        break;
                    case (int)CheckoutEventID.DELETE_ORDER:
                        {

                        }
                        break;
                   default:
                        {

                        }
                        break;
                }
            }
            catch (Exception ex) {
                string err = "MainServices: " + ex.ToString();
                Console.WriteLine(err);
                logging_service.LoggingAppOutput(err, true, true);
            }
        }
        private async Task CreateOrder(string order_detail_id)
        {
            try
            {
                var order = await orderDetailMongoDbModel.FindById(order_detail_id);
                if (order == null || order.carts==null || order.carts.Count<=0) {
                    return;
                }
                Order order_summit = new Order();
                List<OrderDetail> details = new List<OrderDetail>();
                double total_price = 0;
                double total_profit = 0;
                double total_discount = 0;
                double total_amount = 0;
                foreach (var cart in order.carts) {
                    string name_url = CommonHelpers.RemoveUnicode(cart.product.name);
                    name_url = CommonHelpers.RemoveSpecialCharacters(name_url);
                    name_url = name_url.Replace(" ", "-").Trim();
                    details.Add(new OrderDetail()
                    { 
                        CreatedDate=DateTime.Now,
                        Discount=cart.product.discount,
                        OrderDetailId=0,
                        OrderId=0,
                        Price=cart.product.price,
                        Profit = cart.product.profit,
                        Quantity = cart.quanity,
                        Amount = cart.product.amount,
                        ProductCode = cart.product.code,
                        ProductId=cart.product._id,
                        ProductLink = _configuration["Setting:Domain"] + "/san-pham/"+ name_url + "--"+ cart.product._id,
                        TotalPrice = cart.product.price * cart.quanity,
                        TotalProfit = cart.product.profit * cart.quanity,
                        TotalAmount =cart.product.amount * cart.quanity,
                        TotalDiscount= cart.product.discount * cart.quanity,
                        UpdatedDate= DateTime.Now,
                        UserCreate = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                        UserUpdated= Convert.ToInt32(_configuration["Setting:BOT_UserID"])
                    });
                    total_price += (cart.product.price * cart.quanity);
                    total_profit += (cart.product.profit * cart.quanity);
                    total_discount += (cart.product.discount * cart.quanity);
                    total_amount += (cart.product.amount * cart.quanity);
                    cart.total_price = cart.product.price * cart.quanity;
                    cart.total_discount = cart.product.discount * cart.quanity;
                    cart.total_profit = cart.product.profit * cart.quanity;
                    cart.total_amount = cart.product.amount * cart.quanity;
                }
                var account_client=accountClientESService.GetById(order.account_client_id);
                var client = clientESService.GetById((long)account_client.clientid);
               
                order_summit = new Order()
                {
                    Amount = total_amount,
                    ClientId = (long)account_client.clientid,
                    CreatedDate=DateTime.Now,
                    Discount=total_discount,
                    IsDelete=0,
                    Note="",
                    OrderId=0,
                    OrderNo=order.order_no,
                    PaymentStatus=0,
                    PaymentType=Convert.ToInt16(order.payment_type),
                    Price=total_price,
                    Profit=total_profit,
                    OrderStatus=0,
                    UpdateLast=DateTime.Now,
                    UserGroupIds="",
                    UserId = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    UtmMedium=order.utm_medium,
                    UtmSource=order.utm_source,
                    VoucherId=order.voucher_id,
                    CreatedBy = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    UserUpdateId = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    Address=order.address,
                    ReceiverName=order.receivername,
                    Phone=order.phone
                };
                var provinces = _redisService.Get(CacheType.PROVINCE, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                var districts = _redisService.Get(CacheType.DISTRICT, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                var wards = _redisService.Get(CacheType.WARD, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                if (order.provinceid != null && order.provinceid.Trim() != "" && provinces != null && provinces.Trim() != "")
                {
                    var data = JsonConvert.DeserializeObject<List<Province>>(provinces);
                    var province = data.FirstOrDefault(x => x.ProvinceId == order.provinceid);
                    order_summit.ProvinceId = province != null ? province.Id : null;
                }
                if (order.districtid != null && order.districtid.Trim() != "" && districts != null && districts.Trim() != "")
                {
                    var data = JsonConvert.DeserializeObject<List<District>>(districts);
                    var district = data.FirstOrDefault(x => x.DistrictId == order.districtid);
                    order_summit.DistrictId = district != null ? district.Id : null;
                }
                if (order.wardid != null && order.wardid.Trim() != "" && wards != null && wards.Trim() != "")
                {
                    var data = JsonConvert.DeserializeObject<List<Ward>>(wards);
                    var ward = data.FirstOrDefault(x => x.WardId == order.wardid);
                    order_summit.WardId = ward != null ? ward.Id : null;
                }
                var order_id = await orderDAL.CreateOrder(order_summit);
                Console.WriteLine("Created Order - " + order.order_no+": "+ order_id);
                logging_service.LoggingAppOutput("Order Created - " + order.order_no + " - " + total_amount, true, false);

                if (order_id > 0)
                {
                    foreach (var detail in details)
                    {   
                        detail.OrderId = order_id;
                        await orderDetailDAL.CreateOrderDetail(detail);
                        Console.WriteLine("Created OrderDetail - " + detail.OrderId + ": " + detail.OrderDetailId);
                        logging_service.LoggingAppOutput("OrderDetail Created - " + detail.OrderId + ": " + detail.OrderDetailId, true, false);
                        order.order_id=order_id;
                        order.total_price = total_price;
                        order.total_profit=total_profit;
                        order.total_amount= total_amount;
                        order.total_discount= total_discount;
                        await orderDetailMongoDbModel.Update(order);
                    }

                }

            }
            catch (Exception ex)
            {
                string err = "CreateOrder with ["+ order_detail_id+"] error: " + ex.ToString();
                Console.WriteLine(err);
                logging_service.LoggingAppOutput(err, true, true);

            }
        }
    }
}

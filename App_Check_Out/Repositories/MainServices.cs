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
using APP_CHECKOUT.Models.Location;
using Caching.RedisWorker;
using Utilities.Contants;
using DAL;

namespace APP_CHECKOUT.Repositories
{
    public class MainServices: IMainServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggingService logging_service;
        private readonly OrderMongodbService orderDetailMongoDbModel;
        private readonly OrderDAL orderDAL;
        private readonly LocationDAL locationDAL;
        private readonly OrderDetailDAL orderDetailDAL;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly AddressClientESService addressClientESService;
        private readonly NhanhVnService nhanhVnService;
        private readonly RedisConn _redisService;
        public MainServices(IConfiguration configuration, ILoggingService loggingService) {

            _configuration=configuration;
            logging_service=loggingService;
            orderDetailMongoDbModel = new OrderMongodbService(configuration);
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            orderDAL = new OrderDAL(configuration["ConnectionString"]);
            locationDAL = new LocationDAL(configuration["ConnectionString"]);
            orderDetailDAL = new OrderDetailDAL(configuration["ConnectionString"]);
            accountClientESService = new AccountClientESService(configuration["Elastic:Host"], configuration);
            clientESService = new ClientESService(configuration["Elastic:Host"], configuration);
            addressClientESService = new AddressClientESService(configuration["Elastic:Host"], configuration);
            nhanhVnService = new NhanhVnService( configuration,logging_service);
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
                if (order == null || order.carts == null || order.carts.Count <= 0)
                {
                    return;
                }
                Order order_summit = new Order();
                List<OrderDetail> details = new List<OrderDetail>();
                double total_price = 0;
                double total_profit = 0;
                double total_discount = 0;
                double total_amount = 0;
                float total_weight = 0;
                foreach (var cart in order.carts)
                {
                    string name_url = CommonHelpers.RemoveUnicode(cart.product.name);
                    name_url = CommonHelpers.RemoveSpecialCharacters(name_url);
                    name_url = name_url.Replace(" ", "-").Trim();
                    details.Add(new OrderDetail()
                    {
                        CreatedDate = DateTime.Now,
                        Discount = cart.product.discount,
                        OrderDetailId = 0,
                        OrderId = 0,
                        Price = cart.product.price,
                        Profit = cart.product.profit,
                        Quantity = cart.quanity,
                        Amount = cart.product.amount,
                        ProductCode = cart.product.code,
                        ProductId = cart.product._id,
                        ProductLink = _configuration["Setting:Domain"] + "/san-pham/" + name_url + "--" + cart.product._id,
                        TotalPrice = cart.product.price * cart.quanity,
                        TotalProfit = cart.product.profit * cart.quanity,
                        TotalAmount = cart.product.amount * cart.quanity,
                        TotalDiscount = cart.product.discount * cart.quanity,
                        UpdatedDate = DateTime.Now,
                        UserCreate = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                        UserUpdated = Convert.ToInt32(_configuration["Setting:BOT_UserID"])
                    });
                    total_price += (cart.product.price * cart.quanity);
                    total_profit += (cart.product.profit * cart.quanity);
                    total_discount += (cart.product.discount * cart.quanity);
                    total_amount += (cart.product.amount * cart.quanity);
                    cart.total_price = cart.product.price * cart.quanity;
                    cart.total_discount = cart.product.discount * cart.quanity;
                    cart.total_profit = cart.product.profit * cart.quanity;
                    cart.total_amount = cart.product.amount * cart.quanity;
                    total_weight += ((cart.product.weight == null ? 0 : (float)cart.product.weight) * cart.quanity / 1000);

                }
                var account_client = accountClientESService.GetById(order.account_client_id);
                var client = clientESService.GetById((long)account_client.clientid);
                var address_client = addressClientESService.GetById(order.address_id, client.Id);

                order_summit = new Order()
                {
                    Amount = total_amount + order.shipping_fee,
                    ClientId = (long)account_client.clientid,
                    CreatedDate = DateTime.Now,
                    Discount = total_discount,
                    IsDelete = 0,
                    Note = "",
                    OrderId = 0,
                    OrderNo = order.order_no,
                    PaymentStatus = 0,
                    PaymentType = Convert.ToInt16(order.payment_type),
                    Price = total_price,
                    Profit = total_profit,
                    OrderStatus = 0,
                    UpdateLast = DateTime.Now,
                    UserGroupIds = "",
                    UserId = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    UtmMedium = order.utm_medium,
                    UtmSource = order.utm_source,
                    VoucherId = order.voucher_id,
                    CreatedBy = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    UserUpdateId = Convert.ToInt32(_configuration["Setting:BOT_UserID"]),
                    Address = order.address,
                    ReceiverName = order.receivername,
                    Phone = order.phone,
                    ShippingFee = order.shipping_fee,
                    CarrierId = order.delivery_detail.carrier_id,
                    ShippingCode = "",
                    ShippingType = order.delivery_detail.shipping_type,
                    ShippingStatus = 0,
                    PackageWeight = total_weight

                };
                List<Province> provinces = GetProvince();
                List<District> districts = GetDistrict();
                List<Ward> wards = GetWards();
                if (address_client != null && address_client.provinceid != null && address_client.districtid != null && address_client.wardid != null)
                {
                    if (address_client.provinceid.Trim() != "" && provinces != null && provinces.Count > 0)
                    {
                        var province = provinces.FirstOrDefault(x => x.ProvinceId == address_client.provinceid);
                        order_summit.ProvinceId = province != null ? province.Id : null;
                    }
                    if (address_client.districtid.Trim() != "" && districts != null && districts.Count > 0)
                    {
                        var district = districts.FirstOrDefault(x => x.DistrictId == address_client.districtid);
                        order_summit.DistrictId = district != null ? district.Id : null;
                    }
                    if (address_client.wardid.Trim() != "" && wards != null && wards.Count > 0)
                    {
                        var ward = wards.FirstOrDefault(x => x.WardId == address_client.wardid);
                        order_summit.WardId = ward != null ? ward.Id : null;
                    }
                    order_summit.ReceiverName = address_client.receivername;
                    order_summit.Phone = address_client.phone;
                    order_summit.Address = address_client.address;
                }
                else
                {
                    var province = provinces.FirstOrDefault(x => x.ProvinceId == order.provinceid);
                    order_summit.ProvinceId = province != null ? province.Id : null;
                    var district = districts.FirstOrDefault(x => x.DistrictId == order.districtid);
                    order_summit.DistrictId = district != null ? district.Id : null;
                    var ward = wards.FirstOrDefault(x => x.WardId == order.wardid);
                    order_summit.WardId = ward != null ? ward.Id : null;
                    order_summit.ReceiverName = order.receivername;
                    order_summit.Phone = order.phone;
                    order_summit.Address = order.address;
                }
               

                var order_id = await orderDAL.CreateOrder(order_summit);
                Console.WriteLine("Created Order - " + order.order_no+": "+ order_id);
                logging_service.LoggingAppOutput("Order Created - " + order.order_no + " - " + total_amount, true, true);

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
                    await nhanhVnService.PostToNhanhVN(order_summit,order, client, address_client);
                }

            }
            catch (Exception ex)
            {
                string err = "CreateOrder with ["+ order_detail_id+"] error: " + ex.ToString();
                Console.WriteLine(err);
                logging_service.LoggingAppOutput(err, true, true);

            }
        }
        private List<Province> GetProvince()
        {
            List<Province> provinces = new List<Province>();
            string provinces_string = "";

            try
            {
                try
                {
                    provinces_string = _redisService.Get(CacheType.PROVINCE, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                } catch{}
                if (provinces_string == null || provinces_string.Trim() == "")
                {
                    provinces = locationDAL.GetListProvinces();
                    try
                    {
                        _redisService.Set(CacheType.PROVINCE, JsonConvert.SerializeObject(provinces), Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    }catch{}
                }
                else
                {
                    provinces = JsonConvert.DeserializeObject<List<Province>>(provinces_string);
                }
            }
            catch
            {

            }
            return provinces;
        }
        private List<District> GetDistrict()
        {
            List<District> districts = new List<District>();
            string districts_string = "";

            try
            {
                try
                {
                    districts_string = _redisService.Get(CacheType.DISTRICT, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                }
                catch { }
                if (districts_string == null || districts_string.Trim() == "")
                {
                    districts = locationDAL.GetListDistrict();
                    try
                    {
                        _redisService.Set(CacheType.DISTRICT, JsonConvert.SerializeObject(districts), Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    }
                    catch { }
                }
                else
                {
                    districts = JsonConvert.DeserializeObject<List<District>>(districts_string);
                }
            }
            catch
            {

            }
            return districts;
        }
        private List<Ward> GetWards()
        {
            List<Ward> wards = new List<Ward>();
            string wards_string = "";

            try
            {
                try
                {
                    wards_string = _redisService.Get(CacheType.WARD, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                }
                catch { }
                if (wards_string == null || wards_string.Trim() == "")
                {
                    wards = locationDAL.GetListWard();
                    try
                    {
                        _redisService.Set(CacheType.WARD, JsonConvert.SerializeObject(wards), Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    }
                    catch { }
                }
                else
                {
                    wards = JsonConvert.DeserializeObject<List<Ward>>(wards_string);
                }
            }
            catch
            {

            }
            return wards;
        }
        
    }
}

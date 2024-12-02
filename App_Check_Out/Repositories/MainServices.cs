﻿using APP.READ_MESSAGES.Libraries;
using APP_CHECKOUT.DAL;
using APP_CHECKOUT.Helpers;
using APP_CHECKOUT.Interfaces;
using APP_CHECKOUT.MongoDb;
using Entities.Models;
using APP_CHECKOUT.Models.Models.Queue;
using APP_CHECKOUT.Utilities.constants;
using APP_CHECKOUT.Models.Location;
using Utilities.Contants;
using DAL;
using APP_CHECKOUT.RabitMQ;
using System.Configuration;
using Caching.Elasticsearch;
using Newtonsoft.Json;

namespace APP_CHECKOUT.Repositories
{
    public class MainServices: IMainServices
    {
        private readonly ILoggingService logging_service;
        private readonly OrderMongodbService orderDetailMongoDbModel;
        private readonly OrderDAL orderDAL;
        private readonly LocationDAL locationDAL;
        private readonly OrderDetailDAL orderDetailDAL;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly AddressClientESService addressClientESService;
        private readonly NhanhVnService nhanhVnService;
        private readonly WorkQueueClient workQueueClient;

        public MainServices( ILoggingService loggingService) {

            logging_service=loggingService;
            orderDetailMongoDbModel = new OrderMongodbService();
            orderDAL = new OrderDAL(ConfigurationManager.AppSettings["ConnectionString"]);
            locationDAL = new LocationDAL(ConfigurationManager.AppSettings["ConnectionString"]);
            orderDetailDAL = new OrderDetailDAL(ConfigurationManager.AppSettings["ConnectionString"]);
            accountClientESService = new AccountClientESService(ConfigurationManager.AppSettings["Elastic_Host"]);
            clientESService = new ClientESService(ConfigurationManager.AppSettings["Elastic_Host"]);
            addressClientESService = new AddressClientESService(ConfigurationManager.AppSettings["Elastic_Host"]);
            nhanhVnService = new NhanhVnService(logging_service);
            workQueueClient = new WorkQueueClient( loggingService);
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
                logging_service.InsertLogTelegramDirect(err);
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
                        ProductLink = ConfigurationManager.AppSettings["Setting_Domain"] + "/san-pham/" + name_url + "--" + cart.product._id,
                        TotalPrice = cart.product.price * cart.quanity,
                        TotalProfit = cart.product.profit * cart.quanity,
                        TotalAmount = cart.product.amount * cart.quanity,
                        TotalDiscount = cart.product.discount * cart.quanity,
                        UpdatedDate = DateTime.Now,
                        UserCreate = Convert.ToInt32(ConfigurationManager.AppSettings["BOT_UserID"]),
                        UserUpdated = Convert.ToInt32(ConfigurationManager.AppSettings["BOT_UserID"])
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
                //logging_service.InsertLogTelegramDirect(" accountClientESService.GetById("+ order.account_client_id + ") : "+ (account_client == null ? "NULL" : JsonConvert.SerializeObject(account_client)));

                var client = clientESService.GetById((long)account_client.ClientId);
               // logging_service.InsertLogTelegramDirect(" clientESService.GetById(" + (long)account_client.ClientId + ") : " + (client == null ? "NULL" : JsonConvert.SerializeObject(client)));

                AddressClientESModel address_client = addressClientESService.GetById(order.address_id, client.Id);
               // logging_service.InsertLogTelegramDirect(" addressClientESService.GetById(" + order.address_id + "," + client.Id + ") : " + (address_client == null ? "NULL" : JsonConvert.SerializeObject(address_client)));

                order_summit = new Order()
                {
                    Amount = total_amount + order.shipping_fee,
                    ClientId = (long)account_client.ClientId,
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
                    UserId = Convert.ToInt32(ConfigurationManager.AppSettings["BOT_UserID"]),
                    UtmMedium = order.utm_medium,
                    UtmSource = order.utm_source,
                    VoucherId = order.voucher_id,
                    CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["BOT_UserID"]),
                    UserUpdateId = Convert.ToInt32(ConfigurationManager.AppSettings["BOT_UserID"]),
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
                if (address_client != null && address_client.ProvinceId != null && address_client.DistrictId != null && address_client.WardId != null)
                {
                    if (address_client.ProvinceId.Trim() != "" && provinces != null && provinces.Count > 0)
                    {
                        var province = provinces.FirstOrDefault(x => x.ProvinceId == address_client.ProvinceId);
                        order_summit.ProvinceId = province != null ? province.Id : null;
                    }
                    if (address_client.DistrictId.Trim() != "" && districts != null && districts.Count > 0)
                    {
                        var district = districts.FirstOrDefault(x => x.DistrictId == address_client.DistrictId);
                        order_summit.DistrictId = district != null ? district.Id : null;
                    }
                    if (address_client.WardId.Trim() != "" && wards != null && wards.Count > 0)
                    {
                        var ward = wards.FirstOrDefault(x => x.WardId == address_client.WardId);
                        order_summit.WardId = ward != null ? ward.Id : null;
                    }
                    order_summit.ReceiverName = address_client.ReceiverName;
                    order_summit.Phone = address_client.Phone;
                    order_summit.Address = address_client.Address;
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
               // Console.WriteLine("Created Order - " + order.order_no+": "+ order_id);
                logging_service.InsertLogTelegramDirect("Order Created - " + order.order_no + " - " + total_amount);
                workQueueClient.SyncES(order_id, "SP_GetOrder", "hulotoys_sp_getorder", Convert.ToInt16(ProjectType.HULOTOYS));

                if (order_id > 0)
                {
                    foreach (var detail in details)
                    {   
                        detail.OrderId = order_id;
                        await orderDetailDAL.CreateOrderDetail(detail);
                        Console.WriteLine("Created OrderDetail - " + detail.OrderId + ": " + detail.OrderDetailId);
                        logging_service.InsertLogTelegramDirect("OrderDetail Created - " + detail.OrderId + ": " + detail.OrderDetailId);
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
                logging_service.InsertLogTelegramDirect(err);

            }
        }
        private List<Province> GetProvince()
        {
            List<Province> provinces = new List<Province>();
            string provinces_string = "";

            try
            {
                provinces = locationDAL.GetListProvinces();

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
                districts = locationDAL.GetListDistrict();

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
                wards = locationDAL.GetListWard();

            }
            catch
            {

            }
            return wards;
        }
        
    }
}

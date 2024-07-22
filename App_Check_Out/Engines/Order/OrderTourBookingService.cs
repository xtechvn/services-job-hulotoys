using ADAVIGO_FRONTEND_B2C.Models.Tour.TourBooking;
using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using Entities.ViewModels.Tour;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Engines.Order
{
    public class OrderTourBookingService: IOrderTourBookingService
    {
        public int createOrder(TourSummit order_info, OrderEntities order_queue)
        {
            int success = -1;
            try
            {
                if (order_info.order != null && order_info.order.Id > 0)
                {
                    var orderInfo = Repository.getOrderDetailBigint(order_info.order.Id);
                    if (orderInfo == null)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID=" + order_info.order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.order));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID= " + order_info.order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.order));
                        return -1;
                    }
                }

                if (order_info.order == null)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    return -1;
                }
                //-- Check if Order Valid:
                var dataTable = Repository.CheckIfNewOrderValid(order_info.order);
                if (dataTable.Tables.Count < 3)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }
                if (dataTable.Tables[0].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.order));

                    return -1;
                }
                if (dataTable.Tables[1].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }
                /*
                if (dataTable.Tables[2].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService  - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }*/
                Console.WriteLine("Order New: " + order_info.order.OrderNo);
                //--Contact Client
                var data_create_cclient = Repository.CreateContactClients(order_info.obj_contact_client, (long)order_info.order.AccountClientId);
                if (data_create_cclient <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));

                    return -1;
                }
                else
                {
                    order_info.obj_contact_client.Id = data_create_cclient;
                    order_info.order.ContactClientId = data_create_cclient;
                }

                //-- Order
                order_info.order.CreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]);
                order_info.order.CreateTime = DateTime.Now;
                order_info.order.SalerId = Repository.GetUserAgentByClientId((long)order_info.order.ClientId);

                var data_create_order = Repository.CreateOrder(order_info.order, order_info.obj_contact_client.Id);
                if (data_create_order < 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));

                    return -1;
                }
                Console.WriteLine("Contact Client ID: " + order_info.obj_contact_client.Id);
                User? user = Repository.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHTour);

                order_info.booking.OrderId = order_info.order.Id;
                if (user != null && user.Id > 0)
                {
                    order_info.booking.SalerId = user.Id;
                }
                var data_create_tour = Repository.CreateTour(order_info.booking);
                if (data_create_tour < 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - CreateHotelBooking: Cannot Create TourBooking" + JsonConvert.SerializeObject(order_info.booking));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CreateHotelBooking: Cannot Create TourBooking" + JsonConvert.SerializeObject(order_info.booking));

                    return -1;
                }
                foreach (var packages in order_info.packages)
                {
                    packages.TourId = order_info.booking.Id;
                    var data_create_tour_packages = Repository.CreateTourPackages(packages);
                    if (data_create_tour_packages < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - CreateHotelBooking: Cannot Create TourBooking Packages" + JsonConvert.SerializeObject(order_info.booking));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - CreateHotelBooking: Cannot Create TourBooking Packages" + JsonConvert.SerializeObject(order_info.booking));
                    }
                }
                if (order_info.order != null && order_info.order.Id > 0)
                {
                    var update_operatorr = Repository.UpdateOrderOperator(order_info.order.Id);
                }

                //-- Clear Cache:
                var rs = ClearCache((long)order_info.order.ClientId, (long)order_info.order.AccountClientId).Result;
                Console.WriteLine("Order Created: " + order_info.order.Id + " - " + order_info.order.OrderNo);

                //-- Update Checked Session: not neccessary
                // var rs_2 = UpdateCheckedSession((long)order_info.obj_order.ClientId, order_queue.session_id).Result;
                // Console.WriteLine("Updated Session: " + order_queue.session_id);
                //-- Logging
                //   Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - Created OrderID :\n" + order_info.obj_order.Id+" - Client_ID: "+ order_info.obj_order.ClientId+" - AccountClientID: "+ order_info.obj_order.AccountClientId);

                //-- Up to ElasticSearch:
                // var rs_3 = InsertToElasticSearch(order_info).Result;
                //  Console.WriteLine("Insert to ES: " + rs_3);


                return (int)order_info.order.Id;


            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - createOrder: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - createOrder: " + ex.ToString());

            }
            return success;
        }
        public async Task<TourSummit> TourFromMessage(OrderEntities message)
        {
            TourSummit summit_model = null;
            try
            {
                var tour_info = await GetBookingDataFromAPI(message.booking_id);

                long account_client_id = Convert.ToInt64(message.account_client_id);
                var client = Repository.getClientDetailByAccountClientId(account_client_id);
                summit_model = new TourSummit();
                summit_model.order = new OrderViewModel()
                {
                    AccountClientId= account_client_id,
                    Amount=0,
                    BankCode="",
                    Id=message.order_id,
                    BookingInfo=JsonConvert.SerializeObject(message),
                    BranchCode=0,
                    ClientId= client.ClientId,
                    ContactClientId=0,
                    CreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]),
                    CreateTime = DateTime.Now,
                    Description="",
                    Discount=0,
                    EndDate=DateTime.Now,
                    ExpriryDate=DateTime.Now.AddHours(4),
                    IsFinishPayment=false,
                    Exists_id=0,
                    Label="",
                    Note="",
                    OrderNo=message.order_no,
                    OrderStatus=0,
                    StartDate=DateTime.Now,
                    UserUpdateId = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]),
                    UpdateLast = DateTime.Now,
                    ServiceType=Convert.ToByte(message.service_type),
                    ProductService=message.service_type,
                    Profit=0,
                    PaymentStatus=0,
                    PaymentType=0,
                    SalerId = 0,
                    SalerGroupId = "",
                    SystemType = 2,
                    PercentDecrease = 0,
                };
                string contact_client_name = tour_info[0].contact.firstName + " " + tour_info[0].contact.lastName;
                summit_model.obj_contact_client = new ContactClientViewModel()
                {
                    ClientId=client.ClientId,
                    CreateDate=DateTime.Now,
                    Email=tour_info[0].contact.email.Length >50 ? tour_info[0].contact.email.Substring(0,50): tour_info[0].contact.email,
                    Mobile=tour_info[0].contact.phoneNumber.Length > 50 ? tour_info[0].contact.phoneNumber.Substring(0, 50) : tour_info[0].contact.phoneNumber,
                    Name= contact_client_name.Length > 50 ? contact_client_name.Substring(0, 50) : contact_client_name,
                    OrderId=message.order_id,
                    Id=0
                };
                
                if(tour_info[0].tour_product!= null && tour_info[0].tour_product.Id > 0)
                {
                    var data = tour_info[0].tour_product;
                    var packages = tour_info[0].packages;
                    var adt_price = packages != null && packages.Id > 0 ? packages.AdultPrice : data.Price;
                    var chd_price = packages != null && packages.Id > 0 ? packages.ChildPrice : data.Price;
                  
                   
                    summit_model.packages = new List<ViewModel.Tour.TourPackages>();
                    summit_model.packages.Add(new ViewModel.Tour.TourPackages()
                    {
                        Id=0,
                        Amount= (adt_price * (tour_info[0].guest.adult <= 0 ? 1 : tour_info[0].guest.adult)),
                        AmountBeforeVat= (adt_price * (tour_info[0].guest.adult <= 0 ? 1 : tour_info[0].guest.adult)),
                        AmountVat=0,
                        BasePrice= adt_price,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        UpdatedDate = DateTime.Now,
                        PackageCode= "adt_amount",
                        PackageName= "Người lớn",
                        Profit=0,
                        Quantity=tour_info[0].guest.adult,
                        TourId=0,
                        UnitPrice= (chd_price * (tour_info[0].guest.child <= 0 ? 0 : tour_info[0].guest.child)),
                        Vat=0
                    });
                    summit_model.packages.Add(new ViewModel.Tour.TourPackages()
                    {
                        Id = 0,
                        Amount = (chd_price * (tour_info[0].guest.child <= 0 ? 0 : tour_info[0].guest.child)),
                        AmountBeforeVat = (chd_price * (tour_info[0].guest.child <= 0 ? 0 : tour_info[0].guest.child)),
                        AmountVat = 0,
                        BasePrice = chd_price,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        UpdatedDate = DateTime.Now,
                        PackageCode = "chd_amount",
                        PackageName = "Trẻ em (2-14 tuổi)",
                        Profit = 0,
                        Quantity = tour_info[0].guest.child,
                        TourId = 0,
                        UnitPrice = (chd_price * (tour_info[0].guest.child <= 0 ? 0 : tour_info[0].guest.child)),
                        Vat = 0
                    });
                    summit_model.packages.Add(new ViewModel.Tour.TourPackages()
                    {
                        Id = 0,
                        Amount = 0,
                        AmountBeforeVat =0,
                        AmountVat = 0,
                        BasePrice = data.Price,
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        UpdatedDate = DateTime.Now,
                        PackageCode = "inf_amount",
                        PackageName = "Em bé (0-2 tuổi)",
                        Profit = 0,
                        Quantity = 0,
                        TourId = 0,
                        UnitPrice = 0,
                        Vat = 0
                    });
                    var amount = (adt_price * (tour_info[0].guest.adult <= 0 ? 1 : tour_info[0].guest.adult)) + (chd_price * (tour_info[0].guest.child <= 0 ? 0 : tour_info[0].guest.child));
                    summit_model.booking = new ViewModel.Tour.TourViewModel()
                    {
                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        UpdatedDate = DateTime.Now,
                        Amount = amount,
                        Price = amount,
                        AdditionInfo = data.AdditionInfo,
                        Avatar = data.Avatar,
                        Days = data.Days,
                        EndDate = tour_info[0].start_date.AddDays((int)data.Days),
                        StartDate = tour_info[0].start_date,
                        Id = 0,
                        Image = data.Image,
                        IsDisplayWeb = true,
                        Note = tour_info[0].note,
                        OrderId = message.order_id,
                        OrganizingType = data.OrganizingType,
                        Profit = 0,
                        SalerId = 0,
                        Schedule = data.Schedule,
                        ServiceCode = "",
                        Star = data.Star,
                        Status = 0,
                        StatusOld = 0,
                        SupplierId = data.SupplierId,
                        TourProductId = data.Id,
                        TourType = data.TourType,
                        Commission = 0,
                        OthersAmount = 0
                    };
                    summit_model.order.Amount = summit_model.packages.Sum(x=>x.Amount);
                    summit_model.order.Price = summit_model.order.Amount;
                }

            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderTourBookingService - TourFromMessage: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderTourBookingService - TourFromMessage: " + ex.ToString());

            }
            return summit_model;
        }
        private async Task<List<TourBookingRequest>> GetBookingDataFromAPI(string session_id)
        {
            List<TourBookingRequest> result = new List<TourBookingRequest>();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["GET_TOUR_BOOKING_BY_ID"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    session_id = session_id
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = JsonConvert.DeserializeObject<List<TourBookingRequest>>(resultContent_2["data"].ToString());

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public async Task<OrderInfo> GetOrderInfo(long order_id)
        {
            try
            {
                if ( order_id > 0)
                {
                    var orderInfo = Repository.getOrderDetailBigint(order_id);
                    return orderInfo;
                }

            }
            catch
            {

            }
            return null;
        }
        #region Update Cache and MongoDB
        private async Task<string> ClearCache(long client_id, long account_client_id)
        {
            string result = "-1";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_CLEAR_CACHE"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    cache_name = CacheName.CACHENAME_ORDER_CLIENT_ID + client_id,
                    db_index = CacheName.DB_ORDER_CLIENT,
                    cache_type = CacheType.REMOVE_AND_RE_LOAD,
                    data_load = new
                    {
                        client_id = client_id,
                        source_type = (int)SystemLogSourceID.APP
                    }
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = resultContent_2["status"].ToString();
                }

                client = new HttpClient();
                input = new
                {
                    cache_name = CacheName.ORDER_TOUR_ACCOUNTID + account_client_id,
                    db_index = CacheName.DB_ORDER_CLIENT,
                    cache_type = CacheType.REMOVE,
                    data_load = new
                    {
                        client_id = client_id,
                        source_type = (int)SystemLogSourceID.APP
                    }
                };
                token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt"]);
                content_2 = new FormUrlEncodedContent(new[]
               {
                       new KeyValuePair<string, string>("token", token),
                    });
                response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = resultContent_2["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - ClearCache: " + ex.ToString());
            }
            return result;
        }
        /*
        private async Task<string> InsertToElasticSearch(HotelRentOrderSummit order_info)
        {
            string result = "-1";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_Insert_ES"];
                HttpClient client = new HttpClient();
                var input = JsonConvert.DeserializeObject<OrderElasticsearchViewModel>(JsonConvert.SerializeObject(order_info.obj_order));
                if (input != null && input.id > 0)
                {
                    input.deposit_type = (int)DepositPaymentType.PAYMENT_ORDER;
                    input.GenID();
                }
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt_log"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = resultContent_2["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - UpdateCheckedSession: " + ex.ToString());
            }
            return result;
        }*/
        #endregion
    }
}

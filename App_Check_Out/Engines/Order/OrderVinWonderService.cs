using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using ENTITIES.ViewModels.MongoDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Engines.Order
{
    public class OrderVinWonderService : IOrderVinWonderService
    {
        public int createOrder(VinWonderTicketSummit order_info, OrderEntities order_queue)
        {
            int success = -1;
            try
            {
                if (order_info.order != null && order_info.order.Id > 0)
                {
                    var orderInfo = Repository.getOrderDetailBigint(order_info.order.Id);
                    if (orderInfo == null)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID=" + order_info.order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.order));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID= " + order_info.order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.order));
                        return -1;
                    }
                }

                if (order_info.order == null)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    return -1;
                }
                //-- Check if Order Valid:
                var dataTable = Repository.CheckIfNewOrderValid(order_info.order);
                if (dataTable.Tables.Count < 3)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }
                if (dataTable.Tables[0].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.order));

                    return -1;
                }
                if (dataTable.Tables[1].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }
                if (dataTable.Tables[2].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.order));
                    return -1;
                }
                Console.WriteLine("Order New: " + order_info.order.OrderNo);
                //--Contact Client
                var data_create_cclient = Repository.CreateContactClients(order_info.obj_contact_client, (long)order_info.order.AccountClientId);
                if (data_create_cclient <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));

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
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));

                    return -1;
                }
                Console.WriteLine("Contact Client ID: " + order_info.obj_contact_client.Id);
               // User? user = Repository.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHKS);

                foreach (var vinwonder_booking in order_info.bookings)
                {
                    /*
                    if (user != null && user.Id > 0)
                    {
                        vinwonder_booking.SalerId = user.Id;
                    }*/
                    vinwonder_booking.OrderId = data_create_order;
                    var data_create_booking = Repository.CreateVinWonderBooking(vinwonder_booking);
                    if (data_create_booking < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBooking: Cannot create Booking" + JsonConvert.SerializeObject(order_info));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBooking: Cannot create Booking" + JsonConvert.SerializeObject(order_info));
                        return -1;
                    }
                    vinwonder_booking.ServiceCode = "VINWONDER" + string.Format(String.Format("{0,4:0000}", vinwonder_booking.Id));
                    Repository.UpdateVinWonderBooking(vinwonder_booking);
                    foreach (var ticket in vinwonder_booking.tickets)
                    {
                        ticket.BookingId = data_create_booking;
                        var data_create_ticket = Repository.CreateVinWonderBookingTicket(ticket);
                        if (data_create_ticket < 0)
                        {
                            Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBookingTicket: Cannot create Ticket" + JsonConvert.SerializeObject(order_info));
                            Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBookingTicket: Cannot create Ticket" + JsonConvert.SerializeObject(order_info));
                            return -1;
                        }
                        foreach (var ticket_detail in ticket.ticket_detail)
                        {
                            ticket_detail.BookingTicketId = data_create_ticket;
                            var data_create_ticket_detail = Repository.CreateVinWonderTicketDetail(ticket_detail);
                            if (data_create_ticket_detail < 0)
                            {
                                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderTicketDetail: Cannot create TicketDetail" + JsonConvert.SerializeObject(order_info));
                                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderTicketDetail: Cannot create TicketDetail" + JsonConvert.SerializeObject(order_info));
                                return -1;
                            }
                        }
                    }
                    foreach (var passenger in vinwonder_booking.customers)
                    {
                        passenger.BookingId = data_create_booking;
                        var data_create_ticket_passenger = Repository.CreateVinWonderBookingTicketCustomer(passenger);
                        if (data_create_ticket_passenger < 0)
                        {
                            Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBookingTicketCustomer: Cannot create Customer" + JsonConvert.SerializeObject(order_info));
                            Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService  - CreateVinWonderBookingTicketCustomer: Cannot create Customer" + JsonConvert.SerializeObject(order_info));
                            return -1;
                        }
                    }
                }
                if (order_info.order != null && order_info.order.Id > 0)
                {
                    var update_operatorr = Repository.UpdateOrderOperator(order_info.order.Id);
                }
                success = (int)order_info.order.Id;
                //-- Clear Cache:
                var rs = ClearCache((long)order_info.order.ClientId, (long)order_info.order.AccountClientId).Result;
                Console.WriteLine("Order Created: " + order_info.order.Id + " - " + order_info.order.OrderNo);

            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - createOrder: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - createOrder: " + ex.ToString());

            }
            return success;
        }

        public async Task<VinWonderTicketSummit> VinWonderTicketFromMessage(OrderEntities message)
        {
            VinWonderTicketSummit summit_model = null;
            try
            {
                List<BookingVinWonderMongoDbModel> list_booking_data = await GetBookingDataFromAPI(message.booking_cart_id);
                if (list_booking_data.Count < 0)
                {
                    return summit_model;
                }
                summit_model = new VinWonderTicketSummit()
                {
                    bookings = new List<VinWonderBooking>()
                };


                double sale_price = 0;
                double operator_price = 0;
                double order_amount = 0;
                double order_profit = 0;
                double order_price = 0;
                DateTime? date_time_used = null;
                foreach (var item in list_booking_data)
                {
                    var cart = item.data.cart;
                    if (cart != null && cart.Count > 0)
                    {
                        double total_price = 0;
                        double total_profit = 0;
                        double total_amount = 0;
                        VinWonderBooking booking = new VinWonderBooking()
                        {
                            Id = 0,
                            AdavigoBookingId = item._id,
                            Amount = 0,
                            CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                            CreatedDate = DateTime.Now,
                            Note = "Đơn hàng VinWonder tạo tự động",
                            OrderId = 0,
                            SalerId = 0,
                            SiteCode = cart[0][0].SiteCode.ToString(),
                            SiteName = cart[0][0].SiteName,
                            Status = 0,
                            SupplierId = 0,
                            TotalPrice = 0,
                            TotalProfit = 0,
                            TotalUnitPrice = 0,
                            UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                            UpdatedDate = DateTime.Now,
                            tickets = new List<VinWonderBookingTicket>(),
                            customers = new List<VinWonderBookingTicketCustomer>(),
                            Commission=0,
                            OthersAmount=0
                        };
                        foreach (var cart_item in cart)
                        {
                            
                            foreach (var cart_item_detail in cart_item)
                            {
                                total_price += cart_item_detail.TotalPrice * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1);
                                total_profit += cart_item_detail.Profit * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1);
                                total_amount += cart_item_detail.Amount * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1);
                                if (date_time_used == null)
                                {
                                    date_time_used = DateTime.ParseExact(cart_item_detail.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                }

                                var vinwonder_ticket = new VinWonderBookingTicket()
                                {
                                    Adt = cart_item_detail.PeopleNumObj.adt,
                                    Amount = cart_item_detail.Amount * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1),
                                    BookingId = 0,
                                    Child = cart_item_detail.PeopleNumObj.child,
                                    TotalPrice = cart_item_detail.TotalPrice,
                                    CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                    CreatedDate = DateTime.Now,
                                    UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                    UpdatedDate = DateTime.Now,
                                    DateUsed = DateTime.ParseExact(cart_item_detail.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    Id = 0,
                                    Old = cart_item_detail.PeopleNumObj.old,
                                    Quantity = cart_item_detail.Number,
                                    Profit = cart_item_detail.Profit * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1),
                                    RateCode = cart_item_detail.RateCode,
                                    UnitPrice = cart_item_detail.TotalPrice * (cart_item_detail.Number > 0 ? cart_item_detail.Number : 1),
                                    BasePrice = cart_item_detail.TotalPrice ,
                                    Name=cart_item_detail.Name,
                                    ticket_detail = new List<VinWonderBookingTicketDetail>()
                                };
                                foreach (var cart_item_detail_item in cart_item_detail.Items)
                                {
                                    vinwonder_ticket.ticket_detail.Add(new VinWonderBookingTicketDetail()
                                    {
                                        Availability = cart_item_detail_item.Availability,
                                        BasePrice = cart_item_detail_item.BasePrice,
                                        BookingTicketId = 0,
                                        Code = cart_item_detail_item.Code,
                                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                        CreatedDate = DateTime.Now,
                                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                        UpdatedDate = DateTime.Now,
                                        DateFrom = DateTime.ParseExact(cart_item_detail_item.DateTimeUsed.DateFrom + " " + cart_item_detail_item.DateTimeUsed.TimeStart + ":" + cart_item_detail_item.DateTimeUsed.MinuteStart, "dd/MM/yyyy H:m", null),
                                        DateTo = DateTime.ParseExact(cart_item_detail_item.DateTimeUsed.DateTo + " " + cart_item_detail_item.DateTimeUsed.TimeEnd + ":" + cart_item_detail_item.DateTimeUsed.MinuteEnd, "dd/MM/yyyy H:m", null),
                                        GateCode = cart_item_detail_item.DateTimeUsed.GateCode,
                                        GateName = cart_item_detail_item.DateTimeUsed.GateName,
                                        GroupName = cart_item_detail_item.GroupName,
                                        Name = cart_item_detail_item.Name,
                                        NumberOfUses = cart_item_detail_item.NumberOfUses,
                                        Price = cart_item_detail_item.Price,
                                        Id = 0,
                                        PromotionDiscountPercent = cart_item_detail_item.PromotionDiscountPercent,
                                        PromotionDiscountPrice = cart_item_detail_item.PromotionDiscountPrice,
                                        RateDiscountPercent = cart_item_detail_item.RateDiscountPercent,
                                        RateDiscountPrice = cart_item_detail_item.RateDiscountPrice,
                                        ServiceKey = cart_item_detail_item.ServiceKey,
                                        ShortName = cart_item_detail_item.ShortName,
                                        TotalPrice = cart_item_detail_item.TotalPrice,
                                        TypeName = cart_item_detail_item.TypeName,
                                        TypeCode = cart_item_detail_item.TypeCode,
                                        Vatpercent = cart_item_detail_item.VATPercent,
                                        WeekDays = cart_item_detail_item.DateTimeUsed.WeekDays
                                    });
                                }
                                if (vinwonder_ticket.ticket_detail.Count > 0)
                                {
                                    foreach (var ticket_detail in vinwonder_ticket.ticket_detail)
                                    {
                                        if (ticket_detail.Name == null || ticket_detail.Name.Trim() == "")
                                        {
                                            ticket_detail.Name = ticket_detail.ShortName;
                                        }
                                        ticket_detail.ShortName = CommonHelper.RemoveUnicode(ticket_detail.ShortName);
                                    }
                                }

                                booking.tickets.Add(vinwonder_ticket);
                            }
                        }
                        booking.TotalPrice = total_price;
                        booking.TotalProfit = total_profit;
                        booking.Amount = total_amount;
                        order_price += total_price;
                        order_profit += total_profit;
                        order_amount += total_amount;
                        booking.TotalUnitPrice = total_price;
                        summit_model.bookings.Add(booking);
                    }

                }
                //---- Contact Client:
                summit_model.obj_contact_client = new ContactClientViewModel()
                {
                    ClientId = Convert.ToInt64(message.client_id),
                    CreateDate = DateTime.Now,
                    Email = list_booking_data[0].data.infoContact.email,
                    Mobile = list_booking_data[0].data.infoContact.phone,
                    Name = list_booking_data[0].data.infoContact.firstName + " " + list_booking_data[0].data.infoContact.lastName,
                    OrderId = 0,
                    Id = 0
                };
                //-- Order Detail:
                summit_model.order = new OrderViewModel()
                {
                    Amount = order_amount,
                    Profit = order_profit,
                    ClientId = Convert.ToInt64(message.client_id),
                    ContractId = message.contract_id,
                    CreateTime = DateTime.Now,
                    OrderNo = message.order_no,
                    OrderStatus = 0,
                    ServiceType = (int)ServicesType.VinWonder,
                    BankCode = message.bank_code,
                    PaymentType = Convert.ToInt32(message.payment_type),
                    PaymentNo = null,
                    PaymentDate = null,
                    PaymentStatus = 0,
                    Exists_id = message.order_id,
                    Id = message.order_id,
                    ExpriryDate = DateTime.Now.AddHours(4),
                    ProductService = (int)ServicesType.VinWonder,
                    StartDate = date_time_used == null ? DateTime.Now : new DateTime(((DateTime)date_time_used).Year, ((DateTime)date_time_used).Month, ((DateTime)date_time_used).Day, 0, 0, 0),
                    EndDate = date_time_used == null ? DateTime.Now.AddHours(24) : new DateTime(((DateTime)date_time_used).Year, ((DateTime)date_time_used).Month, ((DateTime)date_time_used).Day, 23, 59, 59),
                    AccountClientId = Convert.ToInt64(message.account_client_id),
                    SalerId = 0,
                    SalerGroupId = "",
                    UserUpdateId = 0,
                    SystemType = 2,
                    PercentDecrease = 0,
                    Price = order_price,
                    Discount = 0,
                    Label = "",
                    Note = ""

                };
                summit_model.voucher = await GetVoucherByName(list_booking_data[0].voucher_name, (long)summit_model.order.AccountClientId, (double)summit_model.order.Amount);
                if (summit_model.voucher != null && summit_model.voucher.voucher_id > 0 && DateTime.ParseExact(summit_model.voucher.expire_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date >= DateTime.Now.Date)
                {
                    summit_model.order.Discount = summit_model.voucher.discount;
                    summit_model.order.PercentDecrease = (int)summit_model.voucher.percent_decrease;
                    summit_model.order.Price = summit_model.voucher.total_order_amount_before;
                    summit_model.order.Amount = summit_model.voucher.total_order_amount_after;
                    summit_model.order.VoucherId = summit_model.voucher.voucher_id;
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderVinWonderService - VinWonderTicketFromMessage: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderVinWonderService - VinWonderTicketFromMessage: " + ex.ToString());

            }
            return summit_model;
        }
        private async Task<List<BookingVinWonderMongoDbModel>> GetBookingDataFromAPI(string cart_id)
        {
            List<BookingVinWonderMongoDbModel> result = new List<BookingVinWonderMongoDbModel>();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_GET_BOOKING_VINWONDER_MONGODB_BY_SESSION"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    id = cart_id
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt_log"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token)
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = JsonConvert.DeserializeObject<List<BookingVinWonderMongoDbModel>>(resultContent_2["data"].ToString());
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        private async Task<VoucherViewModel> GetVoucherByName(string voucher_name, long account_client_id, double total_amount)
        {
            VoucherViewModel result = new VoucherViewModel();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_Get_Voucher"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    voucher_name = voucher_name,
                    user_id = account_client_id,
                    service_id = (int)ServicesType.VinWonder,
                    total_order_amount_before = total_amount
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
                    result = JsonConvert.DeserializeObject<VoucherViewModel>(resultContent_2.ToString());
                    if (result == null || result.status != 0)
                    {
                        //Telegram.pushLog("APP.CHECKOUT_SERVICE - GetVoucherByName: Cannot Get voucher from API [" + url + "] - Token:" + token);
                    }
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - GetVoucherByName: " + ex.ToString());
            }
            return result;
        }
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
                    cache_name = CacheName.ORDER_VINWONDER_ACCOUNTID + account_client_id,
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
    }
}

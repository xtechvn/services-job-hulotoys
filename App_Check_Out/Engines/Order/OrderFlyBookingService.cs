using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.MongoDb;
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
    public class OrderFlyBookingService : IOrderFlyBookingService
    {

        public int createOrder(FlyBookingOrderSummit order_info, OrderEntities order_queue)
        {
            try
            {
                if (order_info.obj_order.Id > 0)
                {
                    var orderInfo = Repository.getOrderDetailBigint(order_info.obj_order.Id);
                    if (orderInfo == null)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Order Not Found / Invalid with [ Order_ID=" + order_info.obj_order.Id + "]" + JsonConvert.SerializeObject(order_info.obj_order));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Order Not Found / Invalid with [ Order_ID= " + order_info.obj_order.Id + "]" + JsonConvert.SerializeObject(order_info.obj_order));
                        return -1;
                    }
                    order_info.obj_order.OrderNo = orderInfo.OrderNo;
                }

                if (order_info.obj_order == null)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - CheckIfNewOrderValid: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - CheckIfNewOrderValid: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    return -1;
                }
                //-- Check if Order Valid:
                var dataTable = Repository.CheckIfNewOrderValid(order_info.obj_order);
                if (dataTable.Tables.Count < 3)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                if (dataTable.Tables[0].AsEnumerable().Count() <= 0)
                {
                    var list = dataTable.Tables[0].AsEnumerable();
                    var list_order = JsonConvert.DeserializeObject<List<APP.CHECKOUT_SERVICE.ViewModel.Order.Order>>(JsonConvert.SerializeObject(list));
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.obj_order));

                    return -1;
                }
                if (dataTable.Tables[1].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                if (dataTable.Tables[2].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                Console.WriteLine("Order New: " + order_info.obj_order.OrderNo);

                //--Contact Client
                var data_create_cclient = Repository.CreateContactClients(order_info.obj_contact_client, (long)order_info.obj_order.AccountClientId);
                if (data_create_cclient <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));

                    return -1;
                }
                else
                {
                    order_info.obj_contact_client.Id = data_create_cclient;
                    order_info.obj_order.ContactClientId = data_create_cclient;

                }
                //-- Order
                order_info.obj_order.CreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]);
                order_info.obj_order.CreateTime = DateTime.Now;
                //var client = Repository.getClientDetailByAccountClientId((long)order_info.obj_order.AccountClientId);
                //order_info.obj_order.SalerId = Repository.GetUserAgentByClientId((long)client.ClientId);

                if(order_info.obj_order.Amount==null || order_info.obj_order.Amount <= 0)
                {
                    order_info.obj_order.Amount = order_info.obj_fly_booking.Sum(x => x.Amount);
                }
                if (order_info.obj_order.Note == null) order_info.obj_order.Note = "";
               var data_create_order = Repository.CreateOrder(order_info);
                if (data_create_order < 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));

                    return -1;
                }
               
                Console.WriteLine("Contact Client ID: " + order_info.obj_contact_client.Id);
               
                List<long> group_fly_id = new List<long>();
                User? user = Repository.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHVe);
               
                foreach (var fly_booking in order_info.obj_fly_booking)
                {
                    
                    var airline =  Repository.getAirlinesCodes(new List<String>() { fly_booking.Airline });
                    if(airline!=null && airline.Count > 0)
                    {
                        fly_booking.SupplierId = airline[0].SupplierId;
                    }
                    fly_booking.OrderId = (int)order_info.obj_order.Id;
                    fly_booking.Adgcommission = 0;
                    fly_booking.OthersAmount = 0;
                    //-- FlyBooking Detail
                    var data_create_fly_book_detail = Repository.CreateFlyBookingDetail(fly_booking);
                    if (data_create_fly_book_detail < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Create FlyBookingDetail" + JsonConvert.SerializeObject(fly_booking));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Create FlyBookingDetail" + JsonConvert.SerializeObject(fly_booking));

                        return -1;
                    }
                    group_fly_id.Add(fly_booking.Id);
                    foreach (var seg in fly_booking.segments)
                    {
                        seg.FlyBookingId = fly_booking.Id;
                        var data_create_fly_segment = Repository.CreateFlySegment(seg);
                        if (data_create_fly_segment < 0)
                        {
                            Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Create FlightSegment" + JsonConvert.SerializeObject(seg));
                            Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Create FlightSegment" + JsonConvert.SerializeObject(seg));
                            return -1;
                        }
                    }

                }
                foreach (var fly_booking in order_info.obj_fly_booking)
                {
                    if (user != null && user.Id > 0)
                    {
                        fly_booking.SalerId = user.Id;
                    }
                    fly_booking.UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]);
                    fly_booking.UpdatedDate = DateTime.Now;
                    fly_booking.GroupBookingId = string.Join(",", group_fly_id);
                    fly_booking.Note = order_info.additional.note_go + "\n" + order_info.additional.note_back;
                    var id= Repository.UpdateFlyBookingDetail(fly_booking);

                }
                //-- Passengers:
                foreach (var passenger in order_info.passengers)
                {
                    passenger.OrderId = (long)order_info.obj_order.Id;
                    passenger.GroupBookingId = string.Join(",", group_fly_id);
                    var data_create_passenger = Repository.CreatePassengers(passenger);
                    if (data_create_passenger < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Passengers " + JsonConvert.SerializeObject(passenger));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Passengers " + JsonConvert.SerializeObject(passenger));
                        return -1;
                    }
                    if (passenger.baggages != null && passenger.baggages.Count > 0)
                    {
                        foreach (var baggage in passenger.baggages)
                        {
                            baggage.PassengerId = data_create_passenger;
                            var data_create_passenger_baggage = Repository.CreateBaggage(baggage);
                            if (data_create_passenger_baggage < 0)
                            {
                                Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Baggage " + JsonConvert.SerializeObject(baggage));
                                Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Baggage " + JsonConvert.SerializeObject(baggage));
                                //return -1;
                            }
                            else
                            {
                                Console.WriteLine("Created Baggage: " + baggage.Name + "," + baggage.Price + baggage.Currency);
                            }
                            var data_create_passenger_extra = Repository.CreateFlyBookingExtraPackages(baggage, passenger, string.Join(",", group_fly_id), Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]));
                            if (data_create_passenger_baggage < 0)
                            {
                                Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Extra Packages " + JsonConvert.SerializeObject(baggage));
                                Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: Cannot Create Extra Packages " + JsonConvert.SerializeObject(baggage));
                                //return -1;
                            }
                        }
                    }
                }
                if (order_info.obj_order != null && order_info.obj_order.Id > 0)
                {
                    var update_operatorr = Repository.UpdateOrderOperator(order_info.obj_order.Id);
                }
                //-- Clear Cache:
                var rs = ClearCache((long)order_info.obj_order.ClientId).Result;
                var client = Repository.GetClientByAccountClientId((long)order_info.obj_order.AccountClientId);
                if (client.ParentId != null && client.ParentId > 0)
                {
                    var rs2 = ClearCache((long)client.ParentId).Result;
                }
                Console.WriteLine("Order Created: " + order_info.obj_order.Id + " - " + order_info.obj_order.OrderNo);

                //-- Update Checked Session:
                var rs_2 = UpdateCheckedSession((long)order_info.obj_order.ClientId, order_queue.session_id).Result;
                Console.WriteLine("Updated Session: " + order_queue.session_id);
                //-- Up to ElasticSearch:
                //var rs_3 = InsertToElasticSearch(order_info).Result;
                //Console.WriteLine("Insert to ES: " + rs_3);

                return (int)order_info.obj_order.Id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - createOrder: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - createOrder: " + ex.ToString());

                return -1;
            }
        }

        public async Task<FlyBookingOrderSummit> flyBookingOrderFromMessage(OrderEntities data)
        {
            FlyBookingOrderSummit order_summit = null;
            try
            {
                var client = Repository.GetClientByAccountClientId(Convert.ToInt64(data.account_client_id));
                if (client == null || client.ClientId <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE: Không thể lấy được thông tin Client từ input: \n" + JsonConvert.SerializeObject(data));
                    return order_summit;
                }
                data.client_id = client.ClientId.ToString();
                //Telegram.pushLog("APP.CHECKOUT_SERVICE - Input:\n" + JsonConvert.SerializeObject(data));
                order_summit = new FlyBookingOrderSummit();
                List<BookingFlyMongoDbModel> booking_detail = await GetBookingDataFromAPI(data.session_id, Convert.ToInt32(data.account_client_id));
                if (booking_detail == null || booking_detail.Count < 1)
                {
                    //-- booking data failed
                    List<BookingFlyUnsucessMongoDbModel> booking_detail_unsuccess = await GetUnsuccessBookingDataFromAPI(data.session_id, Convert.ToInt32(data.account_client_id));
                    if (booking_detail_unsuccess == null || booking_detail_unsuccess.Count < 1)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE: Không thể lấy được thông tin booking từ input: \n" + JsonConvert.SerializeObject(data));
                    }
                    else
                    {
                        order_summit = ParseDCDataFailed(booking_detail_unsuccess, data);
                    }
                }
                else
                {
                    order_summit = ParseDCDataSuccess(booking_detail, data);
                }
                if(order_summit!=null && order_summit.obj_fly_booking!=null && order_summit.obj_fly_booking.Count > 0)
                {
                    var service_code = await GetServiceCodeNo();
                    foreach(var fly in order_summit.obj_fly_booking)
                    {
                        fly.ServiceCode = service_code;
                        fly.PriceAdt = (fly.FareAdt + fly.TaxAdt + fly.FeeAdt + fly.ServiceFeeAdt) * fly.AdultNumber;
                        fly.PriceChd = (fly.FareChd + fly.TaxChd + fly.FeeChd + fly.ServiceFeeChd) * fly.ChildNumber;
                        fly.PriceInf = (fly.FareInf + fly.TaxInf + fly.FeeInf + fly.ServiceFeeInf) * fly.InfantNumber;
                        fly.Price = fly.Amount - fly.Profit;
                        fly.Status = 0;
                        fly.Note = fly.Leg == 0 ? "Vé máy bay Chiều đi" : "Vé máy bay Chiều về";
                        fly.ProfitAdt = fly.Profit / (double)(fly.AdultNumber + fly.ChildNumber) * fly.AdultNumber;
                        fly.ProfitChd = fly.Profit / (double)(fly.AdultNumber + fly.ChildNumber) * fly.ChildNumber;
                        fly.ProfitInf = 0;
                    }
                }
                if (!string.IsNullOrEmpty(order_summit.voucher.voucher_name) && order_summit.voucher.voucher_name!=null && order_summit.voucher.voucher_name.Trim()!="")
                {
                    order_summit.voucher = await GetVoucherByName(order_summit.voucher.voucher_name.Trim(), (long)order_summit.obj_order.AccountClientId, (double)order_summit.obj_order.Amount);
                    if (order_summit.voucher != null && order_summit.voucher.voucher_id>0 && DateTime.ParseExact(order_summit.voucher.expire_date, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date >= DateTime.Now.Date)
                    {
                        order_summit.obj_order.Discount = order_summit.voucher.discount;
                        order_summit.obj_order.PercentDecrease = (int)order_summit.voucher.percent_decrease;
                        order_summit.obj_order.Price = order_summit.voucher.total_order_amount_before;
                        order_summit.obj_order.Amount = order_summit.voucher.total_order_amount_after;
                        order_summit.obj_order.VoucherId = order_summit.voucher.voucher_id;
                    }
                }
                if (order_summit != null && order_summit.passengers != null && order_summit.passengers.Count > 0)
                {
                    foreach (var passenger in order_summit.passengers)
                    {
                        passenger.Name = CommonHelper.RemoveSpecialCharacter(passenger.Name);
                    }
                }
                if (order_summit != null && order_summit.obj_contact_client != null)
                {
                    order_summit.obj_contact_client.Name = StringHelper.RemoveSpecialCharacters(order_summit.obj_contact_client.Name);
                    order_summit.obj_contact_client.Mobile = StringHelper.RemoveSpecialCharacters(order_summit.obj_contact_client.Mobile);
                    order_summit.obj_contact_client.Email = StringHelper.RemoveSpecialCharacters(order_summit.obj_contact_client.Email,true);
                    order_summit.obj_contact_client.Name = order_summit.obj_contact_client.Name.Length > 50 ? order_summit.obj_contact_client.Name.Substring(0, 50) : order_summit.obj_contact_client.Name;
                    order_summit.obj_contact_client.Mobile = order_summit.obj_contact_client.Mobile.Length > 50 ? order_summit.obj_contact_client.Mobile.Substring(0, 50) : order_summit.obj_contact_client.Mobile;
                    order_summit.obj_contact_client.Email = order_summit.obj_contact_client.Email.Length > 50 ? order_summit.obj_contact_client.Email.Substring(0, 50) : order_summit.obj_contact_client.Email;

                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - orderFromMessage: " + ex.ToString());
            }
            return order_summit;
        }
       

        private async Task<List<BookingFlyMongoDbModel>> GetBookingDataFromAPI(string session_id, int account_client_id)
        {
            List<BookingFlyMongoDbModel> result = new List<BookingFlyMongoDbModel>();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_GET_BOOKING_MONGODB_BY_SESSION"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    session_id = session_id,
                    client_id = account_client_id
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                       new KeyValuePair<string, string>("source_booking_type", "0")
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = JsonConvert.DeserializeObject<List<BookingFlyMongoDbModel>>(resultContent_2["data"].ToString());
                    if (result.Count > 0)
                    {
                        if (result[0].booking_id == 0 && (result[0].booking_data.ListBooking[0].BookingCode == null || result[0].booking_data.ListBooking[0].BookingCode.Trim() == ""))
                        {
                            result = null;
                        }
                    }
                    /* if (result==null || result.Count < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - GetBookingDataFromAPI: Cannot Get Booking Detail from API ["+url+"] - Token:"+token);
                    }*/
                }
            }
            catch (Exception ex)
            {
                //  Telegram.pushLog("APP.CHECKOUT_SERVICE - GetBookingDataFromAPI: " + ex.ToString());
            }
            return result;
        }
      
        private async Task<List<BookingFlyUnsucessMongoDbModel>> GetUnsuccessBookingDataFromAPI(string session_id, int account_client_id)
        {
            List<BookingFlyUnsucessMongoDbModel> result = new List<BookingFlyUnsucessMongoDbModel>();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_GET_BOOKING_MONGODB_BY_SESSION"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    session_id = session_id,
                    client_id = account_client_id
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), ConfigurationManager.AppSettings["key_encrypt"]);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                        new KeyValuePair<string, string>("source_booking_type", "0")
                    });
                var response = await client.PostAsync(url, content_2);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    result = JsonConvert.DeserializeObject<List<BookingFlyUnsucessMongoDbModel>>(resultContent_2["data"].ToString());
                    if (result == null || result.Count < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - GetUnsuccessBookingDataFromAPI: Cannot Get Booking Detail from API [" + url + "] - Token:" + token);
                    }
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - GetUnsuccessBookingDataFromAPI: " + ex.ToString());
            }
            return result;
        }
        public async Task<OrderWithOldPaymentType> UpdatePaymentTypeWithOrderID(OrderEntities order_info)
        {
            try
            {
                var orderInfo_old = Repository.getOrderDetailBigint(order_info.order_id);
                if (orderInfo_old.PaymentType != Convert.ToInt32(order_info.payment_type))
                {
                    var orderInfo = Repository.UpdateOrderPayment(order_info.order_id, Convert.ToInt32(order_info.payment_type));
                    OrderWithOldPaymentType result = JsonConvert.DeserializeObject<OrderWithOldPaymentType>(JsonConvert.SerializeObject(orderInfo));
                    //-- Clear Cache:
                    var rs = ClearCache(Convert.ToInt64(order_info.client_id)).Result;
                    Console.WriteLine("Updated Payment: " + order_info.order_id + " - " + order_info.order_no);
                    if (result != null && result.OrderId > 0)
                    {
                        result.old_payment_type = orderInfo_old.PaymentType;

                    }
                    else
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - UpdatePaymentTypeWithOrderID Order Data null with:\n " + JsonConvert.SerializeObject(orderInfo));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - UpdatePaymentTypeWithOrderID Order Data null with:\n " + JsonConvert.SerializeObject(orderInfo));
                    }
                    Console.WriteLine("APP.CHECKOUT_SERVICE - Done - Result:\n " + result.OrderId + " - " + result.OrderNo);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - UpdatePaymentTypeWithOrderID: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - UpdatePaymentTypeWithOrderID: " + ex.ToString());
            }
            return null;
        }
        /// <summary>
        /// Parse dữ liệu tạo đơn thất bại từ DC thành object để summit
        /// </summary>
        /// <param name="booking_detail_unsuccess"></param>
        /// <returns></returns>
        public FlyBookingOrderSummit ParseDCDataFailed(List<BookingFlyUnsucessMongoDbModel> booking_detail_unsuccess, OrderEntities data)
        {
            FlyBookingOrderSummit order_summit = null;
            try
            {
                
                double amount = 0;
                double profit = 0;
                DateTime min_expriry_date = DateTime.Now.AddMinutes(20);
                DateTime start_date = DateTime.Now;
                DateTime end_date = new DateTime(2023, 01, 01);
                order_summit.obj_fly_booking = new List<FlyBookingDetailViewModel>();
                foreach (var detail in booking_detail_unsuccess)
                {
                    foreach (var list_booking in detail.booking_data.ListBooking)
                    {

                        foreach (var flight_detail in list_booking.ListFareData)
                        {

                            amount += flight_detail.AdavigoPrice.amount;
                            profit += flight_detail.AdavigoPrice.profit;
                            FlyBookingDetailViewModel fly_booking_item = new FlyBookingDetailViewModel()
                            {
                                Airline = flight_detail.Airline,
                                Amount = flight_detail.AdavigoPrice.amount + flight_detail.AdavigoPrice.profit,
                                BookingCode = "",
                                Currency = "VND",
                                Difference = 0,
                                ExpiryDate = DateTime.Now.AddMinutes(20),
                                Flight = "",
                                Session = detail.session_id,
                                PriceId = flight_detail.AdavigoPrice.price_id,
                                AdultNumber = flight_detail.Adt,
                                ChildNumber = flight_detail.Chd,
                                InfantNumber = flight_detail.Inf,
                                FareAdt = (flight_detail.FareAdt * flight_detail.Adt),
                                FareChd = (flight_detail.FareChd * flight_detail.Chd),
                                FareInf = (flight_detail.FareInf * flight_detail.Inf),
                                TaxAdt = (flight_detail.TaxAdt * flight_detail.Adt),
                                TaxChd = (flight_detail.TaxChd * flight_detail.Chd),
                                TaxInf = (flight_detail.TaxInf * flight_detail.Inf),
                                ServiceFeeAdt = (flight_detail.ServiceFeeAdt * flight_detail.Adt),
                                ServiceFeeChd = (flight_detail.ServiceFeeChd * flight_detail.Chd),
                                ServiceFeeInf = (flight_detail.ServiceFeeInf * flight_detail.Inf),
                                FeeAdt = (flight_detail.FeeAdt * flight_detail.Adt),
                                FeeChd = (flight_detail.FeeChd * flight_detail.Chd),
                                FeeInf = (flight_detail.FeeInf * flight_detail.Inf),

                                TotalNetPrice = flight_detail.TotalNetPrice,
                                TotalDiscount = flight_detail.TotalDiscount,
                                TotalCommission = flight_detail.TotalCommission,
                                AmountAdt = (flight_detail.FareAdt + flight_detail.TaxAdt + flight_detail.FeeAdt + flight_detail.ServiceFeeAdt + flight_detail.AdavigoPrice.profit) * flight_detail.Adt,
                                AmountChd = (flight_detail.FareChd + flight_detail.TaxChd + flight_detail.FeeChd + flight_detail.ServiceFeeChd + flight_detail.AdavigoPrice.profit) * flight_detail.Chd,
                                AmountInf = (flight_detail.FareInf + flight_detail.TaxInf + flight_detail.FeeInf + flight_detail.ServiceFeeInf) * flight_detail.Inf,

                                StartPoint = flight_detail.ListFlight[0].StartPoint,
                                EndPoint = flight_detail.ListFlight[0].EndPoint,
                                Leg = flight_detail.Leg,
                                GroupClass = flight_detail.ListFlight[0].GroupClass,
                                StartDate = flight_detail.ListFlight[0].StartDate.LocalDateTime,
                                EndDate = flight_detail.ListFlight[0].EndDate.LocalDateTime,
                                BookingId = detail.booking_data.BookingId,
                                Status = (int)FlyBookingStatus.CREATED,
                                Profit = flight_detail.AdavigoPrice.profit,
                            };
                            order_summit.additional = new FlyBookingOrderSummitAdditional();
                            fly_booking_item.segments = new List<FlyingSegmentViewModel>();
                            foreach (var segment in flight_detail.ListFlight[0].ListSegment)
                            {
                                var seg = JsonConvert.DeserializeObject<FlyingSegmentViewModel>(JsonConvert.SerializeObject(segment));
                                seg.AllowanceBaggageValue = CommonHelper.GetWeightFromString(seg.AllowanceBaggage);
                                seg.HandBaggageValue = CommonHelper.GetWeightFromString(seg.HandBaggage);
                                fly_booking_item.segments.Add(seg);
                            }
                            if (fly_booking_item.Leg == 0)
                            {
                                start_date = flight_detail.ListFlight[0].StartDate.LocalDateTime;
                                end_date = flight_detail.ListFlight[0].EndDate.LocalDateTime;

                            }
                            if (fly_booking_item.Leg == 1)
                            {
                                end_date = flight_detail.ListFlight[0].EndDate.LocalDateTime;
                            }
                            order_summit.obj_fly_booking.Add(fly_booking_item);


                        }
                    }
                    //---- Re-calucate Price from B2C data in case that didnt get Price from DC:
                    if (detail.booking_session != null && detail.booking_session.search != null && detail.booking_session.search.Trim() != "")
                    {
                        try
                        {
                            var model_session = JsonConvert.DeserializeObject<BookingFlySession>(detail.booking_session.search);
                            if (model_session != null && model_session.go != null && model_session.go.AdavigoPrice != null && model_session.go.AdavigoPrice.amount > 0)
                            {
                                amount = model_session.go.Amount + (model_session.back != null ? model_session.back.Amount : 0);
                                profit = model_session.go.Profit + (model_session.back != null ? model_session.back.Profit : 0);
                                foreach (var fly_book_detail in order_summit.obj_fly_booking)
                                {
                                    if (fly_book_detail.Profit > 0 && fly_book_detail.Amount > 0) continue;
                                    if (fly_book_detail.Leg == 0)
                                    {
                                        fly_book_detail.Amount = model_session.go.Amount;
                                        fly_book_detail.Profit = model_session.go.Profit;
                                    }
                                    if (fly_book_detail.Leg == 1)
                                    {
                                        fly_book_detail.Amount = model_session.back.Amount;
                                        fly_book_detail.Profit = model_session.back.Profit;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                string label = "Vé máy bay ";
                if (order_summit.obj_fly_booking.Count > 1)
                {
                    label += "khứ hồi ";
                    label += order_summit.obj_fly_booking[0].StartPoint + " - " + order_summit.obj_fly_booking[0].EndPoint + " \n";
                    var go = order_summit.obj_fly_booking.Where(x => x.Leg == 0).First();
                    var back = order_summit.obj_fly_booking.Where(x => x.Leg == 1).First();
                    label += "Chiều đi: " + go.Airline + " - " + go.BookingCode + " \n";
                    label += "Chiều về: " + back.Airline + " - " + back.BookingCode + " \n";
                }
                else
                {
                    label += "một chiều ";
                    label +=  order_summit.obj_fly_booking[0].StartPoint + " - " + order_summit.obj_fly_booking[0].EndPoint +" \n";
                    var go = order_summit.obj_fly_booking.Where(x => x.Leg == 0).First();
                    label += "Chiều đi: " + go.Airline + " - " + go.BookingCode + " \n";

                }
                //---- Create Order:
                order_summit.obj_order = new OrderViewModel()
                {
                    Amount = amount,
                    Profit = profit,
                    ClientId = Convert.ToInt64(data.client_id),
                    ContractId = data.contract_id,
                    CreateTime = DateTime.Now,
                    OrderNo = data.order_no,
                    OrderStatus = 0,
                    ServiceType = (int)ServicesType.FlyingTicket,
                    BankCode = data.bank_code,
                    PaymentType = Convert.ToInt32(data.payment_type),
                    PaymentNo = null,
                    PaymentDate = null,
                    PaymentStatus = 0,
                    Exists_id = data.order_id,
                    Id = data.order_id,
                    ExpriryDate = min_expriry_date,
                    ProductService = (int)ServicesType.FlyingTicket,
                    StartDate = start_date,
                    EndDate = end_date,
                    AccountClientId=Convert.ToInt64(data.account_client_id),
                    SalerId=0,
                    SalerGroupId="",
                    UserUpdateId=0,
                    SystemType=2,
                    PercentDecrease=0,
                    Price= amount - profit,
                    Discount = 0,
                    Label= label,
                    Note="",
                    UtmMedium = booking_detail_unsuccess[0].utmmedium ?? "",
                    UtmSource = booking_detail_unsuccess[0].utm_source ?? "",
                };
                order_summit.obj_order.SystemType = Common.Common.GetSystemTypeByOrderNo(order_summit.obj_order.OrderNo);

                //----- Create ContactClient:
                order_summit.obj_contact_client = new ContactClientViewModel()
                {
                    ClientId = Convert.ToInt64(data.client_id),
                    CreateDate = DateTime.Now,
                    Email = booking_detail_unsuccess[0].booking_order.Contact.Email,
                    Mobile = booking_detail_unsuccess[0].booking_order.Contact.Phone,
                    Name = booking_detail_unsuccess[0].booking_order.Contact.FirstName + " " + booking_detail_unsuccess[0].booking_order.Contact.LastName,
                    OrderId=data.order_id
                };
                //---- Passenger and Baggage:
                double price_baggages_from = 0;
                double price_baggages_to = 0;
                order_summit.passengers = new List<PassengerViewModel>();
                foreach (var passenger in booking_detail_unsuccess[0].booking_order.ListPassenger)
                {
                    DateTime? birth_day = null;
                    try
                    {
                        if (passenger.Birthday != null && passenger.Birthday.Trim() != "")
                        {
                            birth_day = DateTime.ParseExact(passenger.Birthday, "ddMMyyyy", null);
                        }
                    }
                    catch { }
                    var passenger_model = new PassengerViewModel()
                    {
                        Gender = passenger.Gender,
                        MembershipCard = passenger.Membership,
                        Name = passenger.FirstName + " " + passenger.LastName,
                        Birthday = birth_day,
                        PersonType = passenger.Type,
                    };
                    if (passenger.ListBaggage.Count > 0)
                    {
                        passenger_model.baggages = new List<BaggageViewModel>();
                        foreach (var package in passenger.ListBaggage)
                        {
                            if (package.Airline != null)
                            {
                                passenger_model.baggages.Add(new BaggageViewModel()
                                {
                                    Airline = package.Airline,
                                    Code = package.Code,
                                    Price = package.Price,
                                    Confirmed = package.Confirmed,
                                    Currency = package.Currency,
                                    EndPoint = package.EndPoint,
                                    FlightId = package.FlightId,
                                    Leg = package.Leg,
                                    Name = package.Name,
                                    StartPoint = package.StartPoint,
                                    StatusCode = package.StatusCode,
                                    Value = package.Value,
                                    WeightValue = CommonHelper.GetWeightFromString(package.Value)
                                });
                                if (package.Leg == 0) price_baggages_from += package.Price;
                                else if (package.Leg == 1) price_baggages_to += package.Price;
                            }

                        };
                    }

                    order_summit.passengers.Add(passenger_model);
                }
                foreach (var flybooking in order_summit.obj_fly_booking)
                {
                    if (flybooking.Leg == 0)
                    {
                        flybooking.TotalBaggageFee = price_baggages_from;
                        flybooking.Amount += price_baggages_from;
                        order_summit.additional.note_go = "Hành lý ký gửi chiều đi: \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");
                    }
                    else if (flybooking.Leg == 1)
                    {
                        flybooking.TotalBaggageFee = price_baggages_to;
                        flybooking.Amount += price_baggages_to;
                        order_summit.additional.note_back = "Hành lý ký gửi chiều về: \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");
                    }
                }
                order_summit.voucher = new VoucherViewModel()
                {
                    voucher_name = booking_detail_unsuccess[0].voucher_name
                };
            }
            catch (Exception ex)
            {

            }
            return order_summit;
        }
        /// <summary>
        /// Parse dữ liệu tạo đơn thành công từ DC thành object để summit
        /// </summary>
        /// <param name="booking_detail"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public FlyBookingOrderSummit ParseDCDataSuccess(List<BookingFlyMongoDbModel> booking_detail, OrderEntities data)
        {
            FlyBookingOrderSummit order_summit = new FlyBookingOrderSummit();
            try
            {
                //-- booking data success:
                //---- Flybooking Detail:
                double amount = 0;
                double profit = 0;
                order_summit.obj_fly_booking = new List<FlyBookingDetailViewModel>();
                DateTime start_date = DateTime.Now;
                DateTime end_date = new DateTime(2023, 01, 01);
                foreach (var detail in booking_detail)
                {
                    foreach (var list_booking in detail.booking_data.ListBooking)
                    {
                        if (list_booking.FareData.ListFlight.Count > 0)
                        {
                            foreach (var flight_detail in list_booking.FareData.ListFlight)
                            {
                                if (list_booking.Status.Trim().ToUpper() == "FAIL")
                                {

                                }
                                amount += list_booking.FareData.TotalPrice;
                                profit += list_booking.Profit;
                                FlyBookingDetailViewModel fly_booking_item = new FlyBookingDetailViewModel()
                                {
                                    Airline = list_booking.Airline,
                                    Amount = list_booking.FareData.TotalPrice + list_booking.Profit,
                                    BookingCode = list_booking.BookingCode,
                                    Currency = "VND",
                                    Difference = list_booking.Difference,
                                    ExpiryDate = list_booking.ExpiryDate.LocalDateTime <= DateTime.Now ? DateTime.Now.AddMinutes(20) : list_booking.ExpiryDate.LocalDateTime,
                                    Flight = flight_detail.FlightNumber,
                                    Session = list_booking.Session,
                                    PriceId = list_booking.PriceId,
                                    #region Calucate fee from DC Data:
                                    /*
                                    AdultNumber = list_booking.FareData.Adt,
                                    ChildNumber = list_booking.FareData.Chd,
                                    InfantNumber = list_booking.FareData.Inf,

                                    FareAdt = (list_booking.FareData.FareAdt * list_booking.FareData.Adt),
                                    FareChd = (list_booking.FareData.FareChd * list_booking.FareData.Chd),
                                    FareInf = (list_booking.FareData.FareInf * list_booking.FareData.Inf),
                                    TaxAdt = (list_booking.FareData.TaxAdt * list_booking.FareData.Adt),
                                    TaxChd = (list_booking.FareData.TaxChd * list_booking.FareData.Chd),
                                    TaxInf = (list_booking.FareData.TaxInf * list_booking.FareData.Inf),
                                    ServiceFeeAdt = (list_booking.FareData.ServiceFeeAdt * list_booking.FareData.Adt),
                                    ServiceFeeChd = (list_booking.FareData.ServiceFeeChd * list_booking.FareData.Chd),
                                    ServiceFeeInf = (list_booking.FareData.ServiceFeeInf * list_booking.FareData.Inf),
                                    FeeAdt = (list_booking.FareData.FeeAdt * list_booking.FareData.Adt),
                                    FeeChd = (list_booking.FareData.FeeChd * list_booking.FareData.Chd),
                                    FeeInf = (list_booking.FareData.FeeInf * list_booking.FareData.Inf),

                                    TotalNetPrice = list_booking.FareData.TotalNetPrice,
                                    TotalDiscount = list_booking.FareData.TotalDiscount,
                                    TotalCommission = list_booking.FareData.TotalCommission,
                                    AmountAdt = (list_booking.FareData.FareAdt + list_booking.FareData.TaxAdt + list_booking.FareData.FeeAdt + list_booking.FareData.ServiceFeeAdt) * list_booking.FareData.Adt,
                                    AmountChd = (list_booking.FareData.FareChd + list_booking.FareData.TaxChd + list_booking.FareData.FeeChd + list_booking.FareData.ServiceFeeChd) * list_booking.FareData.Chd,
                                    AmountInf = (list_booking.FareData.FareInf + list_booking.FareData.TaxInf + list_booking.FareData.FeeInf + list_booking.FareData.ServiceFeeInf) * list_booking.FareData.Inf,
                                    */
                                    #endregion

                                    StartPoint = flight_detail.StartPoint,
                                    EndPoint = flight_detail.EndPoint,
                                    Leg = list_booking.Route.Contains("|") ? flight_detail.Leg : list_booking.FareData.Leg,
                                    GroupClass = flight_detail.GroupClass,
                                    StartDate = flight_detail.StartDate.LocalDateTime,
                                    EndDate = flight_detail.EndDate.LocalDateTime,
                                    BookingId = detail.booking_data.BookingId,
                                    Status =(int)FlyBookingStatus.CREATED,
                                    Profit = list_booking.Profit,
                                    
                                };
                                order_summit.additional = new FlyBookingOrderSummitAdditional();
                               
                                fly_booking_item.segments = new List<FlyingSegmentViewModel>();
                                foreach (var segment in flight_detail.ListSegment)
                                {
                                    var seg = JsonConvert.DeserializeObject<FlyingSegmentViewModel>(JsonConvert.SerializeObject(segment));
                                    seg.AllowanceBaggageValue = CommonHelper.GetWeightFromString(seg.AllowanceBaggage);
                                    seg.HandBaggageValue = CommonHelper.GetWeightFromString(seg.HandBaggage);
                                    fly_booking_item.segments.Add(seg);
                                }
                                
                                if (fly_booking_item.Leg == 0)
                                {
                                    start_date = flight_detail.StartDate.LocalDateTime;
                                    end_date = flight_detail.EndDate.LocalDateTime > end_date ? flight_detail.EndDate.LocalDateTime : end_date;
                                }
                                if (fly_booking_item.Leg == 1)
                                {
                                    end_date = flight_detail.EndDate.LocalDateTime > end_date ? flight_detail.EndDate.LocalDateTime : end_date;

                                }
                                order_summit.obj_fly_booking.Add(fly_booking_item);
                            }

                        }
                        //---- Re-calucate Price from B2C data in case that didnt get Price from DC:
                        if (/*detail.booking_data.ListBooking != null && detail.booking_data.ListBooking.Count > 0 && detail.booking_data.ListBooking[0].Status.Trim().ToLower() == "fail"
                                ||*/ detail.booking_session != null && detail.booking_session.search != null && detail.booking_session.search.Trim() != "")
                        {
                            try
                            {
                                var model_session = JsonConvert.DeserializeObject<BookingFlySession>(detail.booking_session.search);
                                if (model_session != null && model_session.go != null && model_session.go.AdavigoPrice != null && model_session.go.AdavigoPrice.amount > 0)
                                {
                                    amount = model_session.go.Amount + (model_session.back != null ? model_session.back.Amount : 0);
                                    profit = model_session.go.Profit + (model_session.back != null ? model_session.back.Profit : 0);
                                    foreach (var fly_book_detail in order_summit.obj_fly_booking)
                                    {
                                        if (fly_book_detail.Leg == 0)
                                        {
                                            fly_book_detail.AdultNumber = model_session.go.Adt;
                                            fly_book_detail.ChildNumber = model_session.go.Chd;
                                            fly_book_detail.InfantNumber = model_session.go.Inf;
                                            fly_book_detail.FareAdt = model_session.go.FareAdt;
                                            fly_book_detail.FareChd = model_session.go.FareChd;
                                            fly_book_detail.FareInf = model_session.go.FareInf;
                                            fly_book_detail.TaxAdt = model_session.go.TaxAdt;
                                            fly_book_detail.TaxChd = model_session.go.TaxChd;
                                            fly_book_detail.TaxInf = model_session.go.TaxInf;
                                            fly_book_detail.FeeAdt = model_session.go.FeeAdt;
                                            fly_book_detail.FeeChd = model_session.go.FeeChd;
                                            fly_book_detail.FeeInf = model_session.go.FeeInf;
                                            fly_book_detail.ServiceFeeAdt = model_session.go.ServiceFeeAdt;
                                            fly_book_detail.ServiceFeeChd = model_session.go.ServiceFeeChd;
                                            fly_book_detail.ServiceFeeInf = model_session.go.ServiceFeeInf;
                                            fly_book_detail.TotalNetPrice = model_session.go.TotalNetPrice;
                                            fly_book_detail.TotalDiscount = model_session.go.TotalDiscount;
                                            fly_book_detail.TotalCommission = model_session.go.TotalCommission;
                                            fly_book_detail.Amount = model_session.go.Amount;
                                            fly_book_detail.Profit = model_session.go.Profit;
                                            fly_book_detail.AmountAdt = (model_session.go.FareAdt + model_session.go.TaxAdt + model_session.go.FeeAdt + model_session.go.ServiceFeeAdt + model_session.go.AdavigoPrice.profit) * model_session.go.Adt;
                                            fly_book_detail.AmountChd = (model_session.go.FareChd + model_session.go.TaxChd + model_session.go.FeeChd + model_session.go.ServiceFeeChd + model_session.go.AdavigoPrice.profit) * model_session.go.Chd;
                                            fly_book_detail.AmountInf = (model_session.go.FareInf + model_session.go.TaxInf + model_session.go.FeeInf + model_session.go.ServiceFeeInf) * model_session.go.Inf;
                                        }
                                        if (fly_book_detail.Leg == 1)
                                        {
                                            fly_book_detail.AdultNumber = model_session.back.Adt;
                                            fly_book_detail.ChildNumber = model_session.back.Chd;
                                            fly_book_detail.InfantNumber = model_session.back.Inf;
                                            fly_book_detail.FareAdt = model_session.back.FareAdt;
                                            fly_book_detail.FareChd = model_session.back.FareChd;
                                            fly_book_detail.FareInf = model_session.back.FareInf;
                                            fly_book_detail.TaxAdt = model_session.back.TaxAdt;
                                            fly_book_detail.TaxChd = model_session.back.TaxChd;
                                            fly_book_detail.TaxInf = model_session.back.TaxInf;
                                            fly_book_detail.FeeAdt = model_session.back.FeeAdt;
                                            fly_book_detail.FeeChd = model_session.back.FeeChd;
                                            fly_book_detail.FeeInf = model_session.back.FeeInf;
                                            fly_book_detail.ServiceFeeAdt = model_session.back.ServiceFeeAdt;
                                            fly_book_detail.ServiceFeeChd = model_session.back.ServiceFeeChd;
                                            fly_book_detail.ServiceFeeInf = model_session.back.ServiceFeeInf;
                                            fly_book_detail.TotalNetPrice = model_session.back.TotalNetPrice;
                                            fly_book_detail.TotalDiscount = model_session.back.TotalDiscount;
                                            fly_book_detail.TotalCommission = model_session.back.TotalCommission;
                                            fly_book_detail.Amount = model_session.back.Amount;
                                            fly_book_detail.Profit = model_session.back.Profit;
                                            fly_book_detail.AmountAdt = (model_session.back.FareAdt + model_session.back.TaxAdt + model_session.back.FeeAdt + model_session.back.ServiceFeeAdt + model_session.go.AdavigoPrice.profit) * model_session.back.Adt;
                                            fly_book_detail.AmountChd = (model_session.back.FareChd + model_session.back.TaxChd + model_session.back.FeeChd + model_session.back.ServiceFeeChd + model_session.go.AdavigoPrice.profit) * model_session.back.Chd;
                                            fly_book_detail.AmountInf = (model_session.back.FareInf + model_session.back.TaxInf + model_session.back.FeeInf + model_session.back.ServiceFeeInf) * model_session.back.Inf;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                //---- Contact Client:
                order_summit.obj_contact_client = new ContactClientViewModel()
                {
                    ClientId = Convert.ToInt64(data.client_id),
                    CreateDate = DateTime.Now,
                    Email = booking_detail[0].booking_order.Contact.Email,
                    Mobile = booking_detail[0].booking_order.Contact.Phone,
                    Name = booking_detail[0].booking_order.Contact.FirstName + " " + booking_detail[0].booking_order.Contact.LastName,
                    OrderId = data.order_id

                };
                double price_baggages_from = 0;
                double price_baggages_to = 0;
                order_summit.passengers = new List<PassengerViewModel>();
                foreach (var passenger in booking_detail[0].booking_order.ListPassenger)
                {
                    DateTime? birth_day = null;
                    try
                    {
                        if (passenger.Birthday != null && passenger.Birthday.Trim() != "")
                        {
                            birth_day = DateTime.ParseExact(passenger.Birthday,"ddMMyyyy",null);
                        }
                    }
                    catch { }
                    var passenger_model = new PassengerViewModel()
                    {
                        Gender = passenger.Gender,
                        MembershipCard = passenger.Membership,
                        Name = passenger.FirstName + " " + passenger.LastName,
                        Birthday = birth_day,
                        PersonType = passenger.Type,
                    };
                    if (passenger.ListBaggage.Count > 0)
                    {
                        passenger_model.baggages = new List<BaggageViewModel>();
                        foreach (var package in passenger.ListBaggage)
                        {
                            if (package.Airline != null)
                            {
                                passenger_model.baggages.Add(new BaggageViewModel()
                                {
                                    Airline = package.Airline,
                                    Code = package.Code,
                                    Price = package.Price,
                                    Confirmed = package.Confirmed,
                                    Currency = package.Currency,
                                    EndPoint = package.EndPoint,
                                    FlightId = package.FlightId,
                                    Leg = package.Leg,
                                    Name = package.Name,
                                    StartPoint = package.StartPoint,
                                    StatusCode = package.StatusCode,
                                    Value = package.Value,
                                    WeightValue = CommonHelper.GetWeightFromString(package.Value)
                                });
                                if (package.Leg == 0) price_baggages_from += package.Price;
                                else if (package.Leg == 1) price_baggages_to += package.Price;
                            }

                        };
                    }
                    order_summit.passengers.Add(passenger_model);
                }
                //-- Baggage Fee:
                DateTime min_expriry_date = DateTime.Now;
                if (order_summit.obj_fly_booking.Count > 0)
                {
                    min_expriry_date = order_summit.obj_fly_booking[0].ExpiryDate;
                    foreach (var flybooking in order_summit.obj_fly_booking)
                    {
                        if (min_expriry_date > flybooking.ExpiryDate)
                        {
                            min_expriry_date = flybooking.ExpiryDate;
                        }
                        if (flybooking.Leg == 0)
                        {
                            flybooking.TotalBaggageFee = price_baggages_from;
                            // flybooking.Amount += price_baggages_from;
                            order_summit.additional.note_go = "Hành lý ký gửi chiều đi (đi kèm sẵn trong vé): \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");

                        }
                        else if (flybooking.Leg == 1)
                        {
                            flybooking.TotalBaggageFee = price_baggages_to;
                            // flybooking.Amount += price_baggages_to;
                            order_summit.additional.note_back = "Hành lý ký gửi chiều về (đi kèm sẵn trong vé): \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");

                        }
                    }
                }
                else
                {
                    foreach (var flybooking in order_summit.obj_fly_booking)
                    {
                        if (flybooking.Leg == 0)
                        {
                            flybooking.TotalBaggageFee = price_baggages_from;
                            // flybooking.Amount += price_baggages_from;
                            order_summit.additional.note_go = "Hành lý ký gửi chiều đi (đi kèm sẵn trong vé): \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");

                        }
                        else if (flybooking.Leg == 1)
                        {
                            flybooking.TotalBaggageFee = price_baggages_to;
                            //  flybooking.Amount += price_baggages_to;
                            order_summit.additional.note_back = "Hành lý ký gửi chiều về (đi kèm sẵn trong vé): \n" + (flybooking.segments[0].HandBaggage != null && flybooking.segments[0].HandBaggage.Trim() != "" ? flybooking.segments[0].HandBaggage + " xách tay" : " ") + (flybooking.segments[0].AllowanceBaggage != null && flybooking.segments[0].AllowanceBaggage.Trim() != "" ? " + " + flybooking.segments[0].AllowanceBaggage + " ký gửi" : "");

                        }
                    }
                    min_expriry_date = DateTime.Now.AddMinutes(20);
                }
                string label = "Vé ";
                if (order_summit.obj_fly_booking.Count > 1)
                {
                    label += "khứ hồi ";
                    label += order_summit.obj_fly_booking[0].StartPoint + " - " + order_summit.obj_fly_booking[0].EndPoint + " \n";
                    var go = order_summit.obj_fly_booking.Where(x => x.Leg == 0).First();
                    var back = order_summit.obj_fly_booking.Where(x => x.Leg == 1).First();
                    label += "Chiều đi: " + go.Airline + " - " + go.BookingCode + " \n";
                    label += "Chiều về: " + back.Airline + " - " + back.BookingCode + " \n";
                }
                else
                {
                    label += "một chiều ";
                    label += order_summit.obj_fly_booking[0].StartPoint + " - " + order_summit.obj_fly_booking[0].EndPoint + " \n";
                    var go = order_summit.obj_fly_booking.Where(x => x.Leg == 0).First();
                    label += "Chiều đi: " + go.Airline + " - " + go.BookingCode + " \n";

                }
                order_summit.obj_order = new OrderViewModel()
                {
                    Amount = amount,
                    Profit = profit,
                    Discount = 0,
                    ClientId = Convert.ToInt64(data.client_id),
                    ContractId = data.contract_id,
                    CreateTime = DateTime.Now,
                    OrderNo = data.order_no,
                    OrderStatus = 0,
                    ServiceType = (int)ServicesType.FlyingTicket,
                    BankCode = data.bank_code,
                    PaymentType = Convert.ToInt32(data.payment_type),
                    PaymentNo = null,
                    PaymentDate = null,
                    PaymentStatus = 0,
                    Exists_id = data.order_id,
                    Id = data.order_id,
                    ExpriryDate = min_expriry_date,
                    StartDate = start_date,
                    EndDate = end_date,
                    AccountClientId = Convert.ToInt64(data.account_client_id),
                    ProductService= (int)ServicesType.FlyingTicket,
                    SalerId = 0,
                    SalerGroupId = "",
                    UserUpdateId = 0,
                    SystemType = 2,
                    Price = amount -profit,
                    Label = label,
                    Note = "",
                    UtmMedium=booking_detail[0].utmmedium??"",
                    UtmSource = booking_detail[0].utm_source ?? "",

                };
                order_summit.obj_order.SystemType = Common.Common.GetSystemTypeByOrderNo(order_summit.obj_order.OrderNo);
                order_summit.voucher = new VoucherViewModel()
                {
                    voucher_name = booking_detail[0].voucher_name
                };
            }
            catch (Exception ex)
            {

            }
            return order_summit;
        }
        #region Update Cache and MongoDB , ES
        private async Task<string> ClearCache(long client_id)
        {
            string result = "-1";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_CLEAR_CACHE"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    cache_name = CacheName.CACHENAME_ORDER_CLIENT_ID + client_id,
                    db_index = Convert.ToInt32(ConfigurationManager.AppSettings["Redis_dbindex"]),
                    cache_type = CacheType.REMOVE,
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
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - ClearCache: " + ex.ToString());
            }
            return result;
        }
        private async Task<string> UpdateCheckedSession(long client_id, string session_id)
        {
            string result = "-1";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_Update_Checked_Mongo"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    session_id = session_id,
                    client_id = client_id
                };
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
        }
        /*
        private async Task<string> InsertToElasticSearch(FlyBookingOrderSummit order_info)
        {
            string result = "-1";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_Insert_ES"];
                HttpClient client = new HttpClient();
                var input =JsonConvert.DeserializeObject<OrderElasticsearchViewModel>(JsonConvert.SerializeObject(order_info.obj_order));
                input.OrderId = order_info.obj_order.Id;
                if (input!=null && input.id > 0)
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
                    service_id=(int)ServicesType.FlyingTicket,
                    total_order_amount_before=total_amount
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
                      //  Telegram.pushLog("APP.CHECKOUT_SERVICE - GetVoucherByName: Cannot Get voucher from API [" + url + "] - Token:" + token);
                    }
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - GetVoucherByName: " + ex.ToString());
            }
            return result;
        }
        private async Task<string> GetServiceCodeNo()
        {
            string result = "";
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["Get_Order_no"];
                HttpClient client = new HttpClient();
                string data = "{\"key\":{\"code_type\": 2, \"service_type\":" + (int)ServicesType.FlyingTicket + "  }}";
                var token = CommonHelper.Encode(data, ConfigurationManager.AppSettings["key_encrypt_log"]);
                var request = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("token",token)
                });
                var response = await client.PostAsync(url, request);
                dynamic resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                var status = (int)resultContent_2.status;
                if (status == (int)ResponseType.SUCCESS)
                {
                    if (resultContent_2.code != null)
                    {
                        return (string)resultContent_2.code;
                    }
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - GetServiceCodeNo: " + ex.ToString());
            }
            return result;
        }
    }
     
}

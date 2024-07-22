using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Elasticsearch;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.MongoDb;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
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
    public class OrderHotelRentsService : IOrderHotelRentsService
    {
        public int createOrder(HotelRentOrderSummit order_info, OrderEntities order_queue)
        {
            try
            {
                if (order_info.obj_order != null && order_info.obj_order.Id > 0)
                {
                    var orderInfo = Repository.getOrderDetailBigint(order_info.obj_order.Id);
                    if (orderInfo == null)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID=" + order_info.obj_order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.obj_order));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - getOrderDetailBigint: Order Not Found / Invalid with [ Order_ID= " + order_info.obj_order.Exists_id + "]" + JsonConvert.SerializeObject(order_info.obj_order));
                        return -1;
                    }
                }

                if (order_info.obj_order == null)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - createOrder: Order Data is NULL with " + JsonConvert.SerializeObject(order_queue));
                    return -1;
                }
                //-- Check if Order Valid:
                var dataTable = Repository.CheckIfNewOrderValid(order_info.obj_order);
                if (dataTable.Tables.Count < 3)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: SP not Excute with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                if (dataTable.Tables[0].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: Order Not Found / Invalid with " + JsonConvert.SerializeObject(order_info.obj_order));

                    return -1;
                }
                if (dataTable.Tables[1].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: ClientID / AccountClient Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                if (dataTable.Tables[2].AsEnumerable().Count() <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CheckIfNewOrderValid: ContractID Incorrect or not Found with " + JsonConvert.SerializeObject(order_info.obj_order));
                    return -1;
                }
                Console.WriteLine("Order New: " + order_info.obj_order.OrderNo);
                //--Contact Client
                var data_create_cclient = Repository.CreateContactClients(order_info.obj_contact_client, (long)order_info.obj_order.AccountClientId);
                if (data_create_cclient <= 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - CheckIfNewOrderValid: Cannot Insert Contact Client" + JsonConvert.SerializeObject(order_info.obj_contact_client));

                    return -1;
                }
                else
                {
                    order_info.obj_contact_client.Id = data_create_cclient;
                    order_info.obj_order.ContactClientId = data_create_cclient;
                }
                RecorrectData(order_info);
                var client = Repository.GetClientByAccountClientId((long)order_info.obj_order.AccountClientId);
                if (client != null && client.ClientId > 0)
                {
                    var list_saler = Repository.GetListSalerByClientId(client.ClientId);
                    if (list_saler != null && list_saler.Count > 0)
                    {
                        order_info.obj_order.SalerId = list_saler[0];
                        list_saler.RemoveAt(0);
                        if (list_saler.Count > 0)
                        {
                            order_info.obj_order.SalerGroupId = string.Join(",", list_saler);
                        }
                    }
                    order_info.obj_order.ClientId = (long)client.ClientId;
                    order_info.obj_order.SalerId = Repository.GetUserAgentByClientId((long)client.ClientId);
                }

                //-- Order
                order_info.obj_order.CreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]);
                order_info.obj_order.CreateTime = DateTime.Now;


                if (order_info.obj_order.Amount == null || order_info.obj_order.Amount <= 0)
                {
                    order_info.obj_order.Amount = order_info.obj_hotel_rent.Sum(x => x.booking.TotalAmount);
                }
                var data_create_order = Repository.CreateOrder(order_info);
                if (data_create_order < 0)
                {
                    Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));
                    Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - createOrder: Cannot Insert Order" + JsonConvert.SerializeObject(order_info));

                    return -1;
                }

                Console.WriteLine("Contact Client ID: " + order_info.obj_contact_client.Id);
                //-- Passengers: change to create HotelGuest:
                /*
                foreach (var passenger in order_info.passengers)
                {
                    passenger.OrderId = (long)order_info.obj_order.Id;
                    var data_create_passenger = Repository.CreatePassengers(passenger);
                    if (data_create_passenger < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - CreatePassengers: Cannot Create Passengers " + JsonConvert.SerializeObject(passenger));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService  - CreatePassengers: Cannot Create Passengers " + JsonConvert.SerializeObject(passenger));
                        return -1;
                    }

                }*/
                User? user = Repository.GetHeadOfDepartmentByRoleID((int)RoleType.TPDHKS);

                foreach (var hotel_rent_item in order_info.obj_hotel_rent)
                {
                    if (user != null && user.Id > 0)
                    {
                        hotel_rent_item.booking.SalerId = user.Id;
                    }
                    //-- Create Hotel Booking
                    var hotel = Repository.GetHotelByHotelId(hotel_rent_item.booking.PropertyId);
                    if (hotel != null && hotel.Count > 0)
                    {
                        hotel_rent_item.booking.SupplierId = hotel[0].SupplierId;
                    }
                    hotel_rent_item.booking.OrderId = (int)order_info.obj_order.Id;
                    hotel_rent_item.booking.CreatedBy = (int)order_info.obj_order.CreatedBy;
                    hotel_rent_item.booking.CreatedDate = (DateTime)order_info.obj_order.CreateTime;

                    var hotel_id = Repository.CreateHotelBooking(hotel_rent_item.booking);
                    if (hotel_id < 0)
                    {
                        Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBooking: Cannot Create HotelBooking" + JsonConvert.SerializeObject(hotel_rent_item.booking));
                        Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBooking: Cannot Create HotelBooking" + JsonConvert.SerializeObject(hotel_rent_item.booking));

                        return -1;
                    }
                    //-- Create Hotel Booking Rooms
                    foreach (var room in hotel_rent_item.rooms)
                    {
                        room.detail.HotelBookingId = hotel_rent_item.booking.Id;
                        var hotel_detail = Repository.GetHotelByHotelId(hotel_rent_item.booking.PropertyId);
                        room.detail.SupplierId = (hotel_detail == null || hotel_detail.Count <= 0 || hotel_detail[0].SupplierId == null) ? 0 : hotel_detail[0].SupplierId;
                        var data_create_hotel_booking = Repository.CreateHotelBookingRooms(room.detail);
                        if (hotel_id < 0)
                        {
                            Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBookingRooms: Cannot Create HotelBookingRooms" + JsonConvert.SerializeObject(room));
                            Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBookingRooms: Cannot Create HotelBookingRooms" + JsonConvert.SerializeObject(room));

                            return -1;
                        }
                        room.rooms_optional.HotelBookingId = room.detail.HotelBookingId;
                        room.rooms_optional.HotelBookingRoomId = room.detail.Id;
                        var hotel_room_optional_id = Repository.CreateHotelBookingRoomsOptional(room.rooms_optional);
                        //----- Create HotelBookingRoomGuest
                        foreach (var guest in room.guests)
                        {
                            guest.HotelBookingId = hotel_rent_item.booking.Id;
                            guest.HotelBookingRoomsID = room.detail.Id;
                            var data_create_room_guest = Repository.CreateHotelGuest(guest);
                            if (hotel_id < 0)
                            {
                                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelGuest: Cannot Create HotelGuest" + JsonConvert.SerializeObject(guest));
                                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelGuest: Cannot Create HotelGuest" + JsonConvert.SerializeObject(guest));

                                return -1;
                            }
                        }
                        //---- Create Hotel Booking Room Rates
                        foreach (var rate in room.rates)
                        {
                            rate.rates.HotelBookingRoomId = room.detail.Id;
                            var hotel_rate_id = Repository.CreateHotelBookingRoomRates(rate.rates);
                            if (hotel_rate_id < 0)
                            {
                                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBookingRoomRates: Cannot Create HotelBookingRoomRate" + JsonConvert.SerializeObject(rate));
                                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - CreateHotelBookingRoomRates: Cannot Create HotelBookingRoomRate" + JsonConvert.SerializeObject(rate));
                                return -1;
                            }
                            rate.rates_optional.HotelBookingRoomOptionalId = room.rooms_optional.Id;
                            rate.rates_optional.HotelBookingRoomRatesId = rate.rates.Id;
                            var data_create_fly_segment = Repository.CreateHotelBookingRoomsRateOptional(rate.rates_optional);
                        }
                    }
                }
                if (order_info.obj_order != null && order_info.obj_order.Id > 0)
                {
                    var update_operatorr = Repository.UpdateOrderOperator(order_info.obj_order.Id);
                }


                //-- Clear Cache:
                var rs = ClearCache((long)order_info.obj_order.ClientId, (long)order_info.obj_order.AccountClientId).Result;
                if (client.ParentId != null && client.ParentId > 0)
                {
                    var Account = Repository.GetDetailAccountClientByClientId((long)client.ParentId);
                    var rs2 = ClearCache((long)client.ParentId, Account.id).Result;
                }
                Console.WriteLine("Order Created: " + order_info.obj_order.Id + " - " + order_info.obj_order.OrderNo);

                //-- Update Checked Session: not neccessary
                // var rs_2 = UpdateCheckedSession((long)order_info.obj_order.ClientId, order_queue.session_id).Result;
                // Console.WriteLine("Updated Session: " + order_queue.session_id);
                //-- Logging
                //   Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - Created OrderID :\n" + order_info.obj_order.Id+" - Client_ID: "+ order_info.obj_order.ClientId+" - AccountClientID: "+ order_info.obj_order.AccountClientId);

                //-- Up to ElasticSearch:
                // var rs_3 = InsertToElasticSearch(order_info).Result;
                //  Console.WriteLine("Insert to ES: " + rs_3);


                return (int)order_info.obj_order.Id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - createOrder: " + ex.ToString());
                Console.WriteLine("APP.CHECKOUT_SERVICE - OrderHotelRentsService - createOrder: " + ex.ToString());

                return -1;
            }
        }
        public void RecorrectData(HotelRentOrderSummit order_info)
        {
            try
            {
                if (order_info.obj_order.PaymentType == (int)PaymentType.KY_QUY)
                {
                    order_info.obj_order.OrderStatus = 2;
                }
                foreach (var hotel in order_info.obj_hotel_rent)
                {
                    DateTime arrival_date = hotel.booking.ArrivalDate;
                    DateTime departure_date = hotel.booking.DepartureDate;

                    foreach (var room in hotel.rooms)
                    {
                        foreach (var rate in room.rates)
                        {
                            if (arrival_date > rate.rates.StartDate)
                            {
                                arrival_date = Convert.ToDateTime(rate.rates.StartDate.ToString());
                            }
                            if (departure_date < rate.rates.EndDate)
                            {
                                departure_date = Convert.ToDateTime(rate.rates.EndDate.ToString());
                            }
                        }
                    }
                    hotel.booking.ArrivalDate = arrival_date;
                    hotel.booking.DepartureDate = departure_date;
                    if (order_info.obj_order.PaymentType == (int)PaymentType.KY_QUY)
                    {
                        hotel.booking.Status = 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
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
                    var rs = ClearCache(Convert.ToInt64(order_info.client_id), orderInfo.AccountClientId).Result;
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
        public async Task<HotelRentOrderSummit> HotelRentSummitFromMessage(OrderEntities message)
        {
            HotelRentOrderSummit order_summit = new HotelRentOrderSummit();

            try
            {
                var data_list = await GetBookingDataFromAPI(message.booking_id, Convert.ToInt64(message.account_client_id));
                if (data_list != null && data_list.Count > 0)
                {
                    order_summit.obj_hotel_rent = new List<HotelRentOrderSummitDetail>();
                    double total_price = 0;
                    double total_profit = 0;
                    double total_amount = 0;

                    foreach (var data in data_list)
                    {
                        double booking_price = 0;
                        double booking_profit = 0;
                        double booking_amount = 0;
                        int total_adt = 0;
                        int total_chd = 0;
                        int total_inf = 0;
                        var rooms = new List<HotelBookingRoomsSummit>();
                        foreach (var room in data.booking_b2b_data.rooms)
                        {
                            total_price += room.price;
                            total_amount += room.total_amount;
                            total_profit += room.profit;
                            booking_price += room.price;
                            booking_profit += room.profit;
                            booking_amount += room.total_amount;
                            total_adt += room.numberOfAdult;
                            total_chd += room.numberOfChild;
                            total_inf += room.numberOfInfant;
                            var room_detail = new HotelBookingRoomsSummit()
                            {
                                detail = new HotelBookingRooms()
                                {
                                    HotelBookingId = -1,
                                    Id = -1,
                                    RoomTypeCode = room.room_type_code ?? "",
                                    RoomTypeId = room.room_type_id ?? "",
                                    RoomTypeName = room.room_type_name ?? "",
                                    TotalAmount = room.total_amount,
                                    Price = room.price,
                                    Profit = room.profit,
                                    NumberOfAdult = room.numberOfAdult,
                                    NumberOfChild = room.numberOfChild,
                                    NumberOfInfant = room.numberOfInfant,
                                    PackageIncludes = room.rates != null && room.rates.Count > 0 && room.rates[0].package_includes != null && room.rates[0].package_includes.Count > 0 ? string.Join(",", room.rates[0].package_includes) : (room.package_includes == null || room.package_includes.Count <= 0 ? "" : string.Join(",", room.package_includes)),
                                    ExtraPackageAmount = 0,
                                    CreatedDate = DateTime.Now,
                                    TotalUnitPrice = room.price,
                                    CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                    NumberOfRooms = room.numberOfRooms ?? 1,
                                    Status = 0,

                                },
                                rates = new List<HotelBookingRoomsSummitRate>(),
                                guests = new List<HotelGuestViewModel>(),
                                rooms_optional = new HotelBookingRoomsOptional(),
                            };
                            foreach (var guest in room.guests)
                            {
                                room_detail.guests.Add(new HotelGuestViewModel()
                                {
                                    Birthday = (guest.birthday == null || guest.birthday.Trim() == "") ? DateTime.Now : DateTime.ParseExact(guest.birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                    HotelBookingRoomsID = -1,
                                    Name = guest.firstName + " " + guest.lastName,
                                    HotelBookingId = -1,
                                    Type = guest.type ?? 0,
                                    Note = guest.note ?? ""
                                });
                            }

                            foreach (var rate in room.rates)
                            {
                                var arrive_date_datetime = DateTime.ParseExact(rate.arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                var departure_date_datetime = DateTime.ParseExact(rate.departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                int stay_date_by_rate = (departure_date_datetime - arrive_date_datetime).Days;
                                room_detail.rates.Add(new HotelBookingRoomsSummitRate()
                                {
                                    rates = new HotelBookingRoomRates()
                                    {
                                        AllotmentId = rate.allotment_id,
                                        HotelBookingRoomId = -1,
                                        Id = -1,
                                        Price = rate.price,
                                        Profit = rate.profit,
                                        RatePlanCode = rate.rate_plan_code,
                                        RatePlanId = rate.rate_plan_id,
                                        StayDate = arrive_date_datetime,
                                        TotalAmount = rate.total_amount,
                                        PackagesInclude = rate.package_includes != null && rate.package_includes.Count > 0 ? string.Join(",", rate.package_includes) : "",
                                        UnitPrice = rate.price - rate.profit,
                                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                        StartDate = arrive_date_datetime,
                                        EndDate = departure_date_datetime,
                                        Nights = (short)stay_date_by_rate,
                                        OperatorPrice = Math.Round(rate.price / (double)stay_date_by_rate, 0),
                                        SalePrice = Math.Round(rate.total_amount / (double)stay_date_by_rate, 0),
                                    },
                                    rates_optional = new HotelBookingRoomRatesOptional()
                                    {
                                        Price = rate.price,
                                        Profit = rate.profit,
                                        TotalAmount = rate.price,
                                        Id = 0,
                                        HotelBookingRoomOptionalId = 0,
                                        UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                        UpdatedDate = DateTime.Now,
                                        CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                        CreatedDate = DateTime.Now,
                                        HotelBookingRoomRatesId = 0,
                                        OperatorPrice = Math.Round(rate.price / (double)stay_date_by_rate, 0),

                                    }

                                });
                            }
                            room_detail.rooms_optional = new HotelBookingRoomsOptional()
                            {
                                CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                CreatedDate = DateTime.Now,
                                HotelBookingId = 0,
                                HotelBookingRoomId = 0,
                                NumberOfRooms = room_detail.detail.NumberOfRooms,
                                Id = 0,
                                Price = room_detail.detail.Price,
                                Profit = room_detail.detail.Profit,
                                SupplierId = room_detail.detail.SupplierId != null ? (int)room_detail.detail.SupplierId : 0,
                                TotalAmount = room_detail.detail.Price,
                                UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                UpdatedDate = DateTime.Now,
                                PackageName = "",
                                IsRoomFund = false
                            };

                            rooms.Add(room_detail);
                        }

                        var booking_item_summit = new HotelRentOrderSummitDetail()
                        {
                            booking = new HotelBooking()
                            {
                                ArrivalDate = DateTime.ParseExact(data.booking_order.arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                DepartureDate = DateTime.ParseExact(data.booking_order.departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                BookingId = data._id,
                                NumberOfAdult = data.booking_order.numberOfAdult,
                                NumberOfChild = data.booking_order.numberOfChild,
                                NumberOfInfant = data.booking_order.numberOfInfant,
                                NumberOfRoom = data.booking_order.numberOfRoom,
                                PropertyId = data.booking_order.hotelID,
                                TotalAmount = booking_amount,
                                Status = 0,
                                HotelName = data.booking_data.propertyName,
                                TotalPrice = booking_price,
                                TotalProfit = booking_profit,
                                Email = data.booking_b2b_data.detail.email == null ? "" : data.booking_b2b_data.detail.email,
                                Telephone = data.booking_b2b_data.detail.telephone == null ? "" : data.booking_b2b_data.detail.telephone,
                                Address = data.booking_b2b_data.detail.address == null ? "" : data.booking_b2b_data.detail.address,
                                ImageThumb = data.booking_b2b_data.detail.image_thumb == null ? "" : data.booking_b2b_data.detail.image_thumb,
                                CheckinTime = data.booking_b2b_data.detail.check_in_time == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 00) : TimeZone.CurrentTimeZone.ToLocalTime(data.booking_b2b_data.detail.check_in_time),
                                CheckoutTime = data.booking_b2b_data.detail.check_out_time == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00) : TimeZone.CurrentTimeZone.ToLocalTime(data.booking_b2b_data.detail.check_out_time),
                                ExtraPackageAmount = 0,
                                SalerId = 0,
                                ServiceCode = await GetServiceCodeNo(),
                                Price = booking_price,
                                UpdatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                UpdatedDate = DateTime.Now,
                                CreatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["Created_By_BotID"]),
                                CreatedDate = DateTime.Now,
                                NumberOfPeople = total_adt + total_chd + total_inf,
                                Note = data.booking_b2b_data.contact.note == null ? "Đơn hàng khách sạn " + data.booking_data.propertyName : data.booking_b2b_data.contact.note,
                                TotalDiscount = 0,
                                TotalOthersAmount = 0,
                                SupplierId = 0,
                                ReservationCode = "",
                                StatusOld = 0,

                            },
                            rooms = rooms,
                        };


                        order_summit.obj_hotel_rent.Add(booking_item_summit);
                    }
                    order_summit.obj_contact_client = new ContactClientViewModel()
                    {
                        ClientId = Convert.ToInt64(message.client_id),
                        CreateDate = DateTime.Now,
                        Email = data_list[0].booking_b2b_data.contact.email,
                        Mobile = data_list[0].booking_b2b_data.contact.phoneNumber,
                        Name = data_list[0].booking_b2b_data.contact.lastName + " " + data_list[0].booking_b2b_data.contact.firstName,
                        Id = -1,
                        OrderId = -1
                    };
                    order_summit.obj_contact_client.Name = order_summit.obj_contact_client.Name.Length > 50 ? order_summit.obj_contact_client.Name.Substring(0, 50) : order_summit.obj_contact_client.Name;
                    order_summit.obj_contact_client.Mobile = order_summit.obj_contact_client.Mobile.Length > 50 ? order_summit.obj_contact_client.Mobile.Substring(0, 50) : order_summit.obj_contact_client.Mobile;
                    order_summit.obj_contact_client.Email = order_summit.obj_contact_client.Email.Length > 50 ? order_summit.obj_contact_client.Email.Substring(0, 50) : order_summit.obj_contact_client.Email;
                    /*
                    order_summit.passengers = new List<PassengerViewModel>();
                    foreach (var p in data_list[0].booking_b2b_data.guests)
                    {
                        order_summit.passengers.Add(new PassengerViewModel()
                        {
                            Birthday = DateTime.ParseExact(p.birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            Gender = true,
                            Name = p.firstName + p.lastName,
                            OrderId = -1,
                            PersonType = ""
                        });
                    };*/

                    int hotel_id = -1;
                    order_summit.obj_order = new OrderViewModel()
                    {
                        Amount = total_amount,
                        Profit = total_profit,
                        Discount = 0,
                        ClientId = Convert.ToInt64(message.client_id),
                        ContractId = null,
                        CreateTime = DateTime.Now,
                        OrderNo = message.order_no,
                        OrderStatus = 0,
                        ServiceType = Convert.ToByte(message.service_type),
                        BankCode = message.bank_code,
                        PaymentType = Convert.ToInt32(message.payment_type),
                        PaymentNo = null,
                        PaymentDate = null,
                        PaymentStatus = 0,
                        Id = message.order_id,
                        Exists_id = message.order_id,
                        ExpriryDate = DateTime.Now.AddHours(1),
                        ProductService =  (int)ServicesType.VINHotelRent,
                        StartDate = DateTime.ParseExact(data_list[0].booking_order.arrivalDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(data_list[0].booking_order.departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        AccountClientId = Convert.ToInt64(data_list[0].account_client_id),
                        SalerId = 0,
                        SalerGroupId = "",
                        UserUpdateId = 0,
                        SystemType = 1,
                        Label = "Đặt phòng Khách sạn " + data_list[0].booking_data.propertyName,
                        Price = total_amount - total_profit,
                        CreatedBy = Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"]),
                        SupplierId = 0,
                        Note = ""


                    };
                    order_summit.obj_order.SystemType = Common.Common.GetSystemTypeByOrderNo(order_summit.obj_order.OrderNo);

                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("APP.CHECKOUT_SERVICE - OrderHotelRentsService - IOrderHotelRentsService: " + ex.ToString());

            }
            return order_summit;
        }
        private async Task<List<BookingHotelMongoViewModel>> GetBookingDataFromAPI(string session_id, long account_client_id)
        {
            List<BookingHotelMongoViewModel> result = new List<BookingHotelMongoViewModel>();
            try
            {
                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_GET_BOOKING_HOTEL_MONGODB_BY_SESSION"];
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
                    result = JsonConvert.DeserializeObject<List<BookingHotelMongoViewModel>>(resultContent_2["data"].ToString());

                }
            }
            catch (Exception ex)
            {

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
                string data = "{\"key\":{\"code_type\": 2, \"service_type\":" + (int)ServicesType.VINHotelRent + "  }}";
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
        #region Update Cache and MongoDB
        public async Task<string> ClearCache(long client_id, long account_client_id)
        {
            string result = "-1";
            try
            {
                string cache_name = CacheType.ORDER_ACCOUNT_CLIENT_HOTEL + account_client_id;

                string url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_CLEAR_CACHE"];
                HttpClient client = new HttpClient();
                var input = new
                {
                    cache_name= cache_name,
                    db_index = CacheName.DB_ORDER_CLIENT_B2B,
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

                client = new HttpClient();
                input = new
                {
                    cache_name = CacheName.ORDER_ACCOUNT_CLIENT_HOTEL + account_client_id,
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

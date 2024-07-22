using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Engines.API;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Mail;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using static APP.CHECKOUT_SERVICE.Contants.CommonConstant;

namespace APP.CHECKOUT_SERVICE.Engines.Mail
{
    public class MailService : IMailService
    {
        /// <summary>
        /// Scope: B2C
        /// Thực hiện gửi mail theo rule của mkt dựa vào order_id
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public bool sendMailOrderB2C(int order_id)
        {
            try
            {
                //2 get template mail
                string projectDirectory = Environment.CurrentDirectory;
                //var currentDirectory = Directory.GetParent(projectDirectory).Parent.Parent.FullName;
                var template = projectDirectory + @"/EmailTemplate/OrderTemplate.html";
                var subject = File.ReadAllText(template);

                if (order_id == -1)
                {
                    Telegram.pushLog("Service sendMailOrderB2C. Create order Fail. order_id = " + order_id);
                    return false;
                }

                //order_id = 43;
                //1. get order info
                var orderInfo = Repository.getOrderDetail(order_id);

                var client = Repository.getClientDetailByAccountClientId(orderInfo != null ? orderInfo.AccountClientId : 0);
                List<FlyBookingDetail> flyBookingDetailList = Repository.getFlyBookingDetailList(orderInfo.OrderId);
                List<FlightSegment> flightSegmentList = Repository.getFlightSegmentList(flyBookingDetailList.Select(n => n.Id).ToList());
                ContactClient contactClient = Repository.getContactClient((int)orderInfo.ContactClientId);
                var listAirportCode = Repository.getAllAirportCode();
                var listAirlines = Repository.getAllAirlines();
                var listPassenger = Repository.getListPassenger(order_id);
                var listPassengerId = listPassenger != null ? listPassenger.Select(n => n.Id).ToList() : new List<int>();
                var listBaggage = new List<Baggage>();
                if (listPassengerId.Count > 0)
                    listBaggage = Repository.getListBaggage(listPassengerId);


                var images_url = "https://static-image.adavigo.com/uploads/images/airlinelogo/";
                //3. fill data to template
                //thông tin order
                subject = subject.Replace("{{orderNo}}", orderInfo.OrderNo);
                subject = subject.Replace("{{orderDate}}", orderInfo.CreateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"));

                var flyBookingDetail = flyBookingDetailList.FirstOrDefault();

                if (flyBookingDetail != null)
                {
                    var expiryDateMin = flyBookingDetail.ExpiryDate;
                    foreach (var item in flyBookingDetailList)
                    {
                        if (item.ExpiryDate < expiryDateMin)
                            expiryDateMin = item.ExpiryDate;
                    }
                    if (flyBookingDetailList.FirstOrDefault(n => string.IsNullOrEmpty(n.BookingCode)) != null)
                    {
                        subject = subject.Replace("{{keepTicketTime}}", "Thời gian thanh toán đến " + expiryDateMin.ToString("HH:mm:ss dd/MM/yyyy"));
                        subject = subject.Replace("{{keepTicketTimeText}}", " <br>Loại vé không giữ được chỗ. Vui lòng liên hệ hotline 093.6191.192 để đặt vé");
                    }
                    else
                    {
                        subject = subject.Replace("{{keepTicketTime}}", "Đang giữ vé đến " + expiryDateMin.ToString("HH:mm:ss dd/MM/yyyy"));
                        subject = subject.Replace("{{keepTicketTimeText}}", "");

                    }
                    //số tiền thanh toán
                    subject = subject.Replace("{{total}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));
                    //số tiền cần thanh toán
                    subject = subject.Replace("{{amount}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));
                }

                //thông tin hành khách
                subject = subject.Replace("{{customerName}}", contactClient?.Name);
                //subject = subject.Replace("{{gender}}", client?.Gender == (int)CommonGender.FEMALE ? "Nữ" : "Nam");
                var mobilephone = (contactClient?.Mobile) == null || (contactClient?.Mobile).StartsWith("0") || (contactClient?.Mobile).StartsWith("+") || (contactClient?.Mobile).StartsWith("(+") ? contactClient?.Mobile : "0" + contactClient?.Mobile;
                subject = subject.Replace("{{phone}}", mobilephone);
                subject = subject.Replace("{{email}}", contactClient?.Email);

                string passenger = String.Empty;
                var count = 1;
                foreach (var item in listPassenger)
                {
                    Baggage baggageGo = listBaggage.FirstOrDefault(n => n.PassengerId == item.Id && n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                    Baggage baggageBack = listBaggage.FirstOrDefault(n => n.PassengerId == item.Id && n.Leg == (int)CommonConstant.FlyBookingDetailType.BACK);
                    var flyBookingGo = flyBookingDetailList.FirstOrDefault(n => n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                    var flightSegmentGo = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingGo?.Id);
                    var flyBookingBack = flyBookingDetailList.FirstOrDefault(n => n.Leg != (int)CommonConstant.FlyBookingDetailType.GO);
                    var flightSegmentBack = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingBack?.Id);

                    string baggeGo = String.Empty;
                    string baggeBack = String.Empty;

                    if (flightSegmentGo != null && flightSegmentGo.AllowanceBaggageValue > 0)
                        baggeGo = " + " + flightSegmentGo?.AllowanceBaggageValue + " kg ký gửi ";

                    if (baggageGo != null && baggageGo.WeightValue > 0)
                        baggeGo = " + " + baggageGo?.WeightValue + " kg ký gửi ";

                    if (baggageGo != null && flightSegmentGo != null && baggageGo.WeightValue > 0 && flightSegmentGo.AllowanceBaggageValue > 0)
                        baggeGo = " + " + (flightSegmentGo?.AllowanceBaggageValue + baggageGo?.WeightValue) + " kg ký gửi ";

                    if (flightSegmentBack != null && flightSegmentBack.AllowanceBaggageValue > 0)
                        baggeBack = " + " + flightSegmentBack?.AllowanceBaggageValue + " kg ký gửi ";

                    if (baggageBack != null && baggageBack.WeightValue > 0)
                        baggeBack = " + " + baggageBack?.WeightValue + " kg ký gửi ";

                    if (baggageBack != null && flightSegmentBack != null && baggageBack.WeightValue > 0 && flightSegmentBack.AllowanceBaggageValue > 0)
                        baggeBack = " + " + (flightSegmentBack?.AllowanceBaggageValue + baggageBack?.WeightValue) + " kg ký gửi ";
                    string birthday = string.Empty;
                    if (item.Birthday != null)
                        birthday = item.Birthday.Value.ToString("dd/MM/yyyy");
                    passenger += @"<tr style=" + "\"" + "font-size: 14px;" + "\"" + ">" +
                                    "<td>" + count + @"</td>" +
                                    "<td><strong>" + item.Name + @"</strong></td>" +
                                    "<td>" + (item.Gender == false ? "Nữ" : "Nam") + @"</td>" +
                                    "<td>" + birthday + @"</td>" +
                                    "<td style=" + "\"" + "font-size: 12px; font-weight: bold;" + "\"" + ">" +
                                        "<p {{displayChieuDi}}> Chiều đi: " + flightSegmentGo?.HandBaggageValue + " kg xách tay " + baggeGo + @"</p>" +
                                        "<p {{displayChieuVe}}> Chiều về: " + flightSegmentBack?.HandBaggageValue + " kg xách tay " + baggeBack + @" </p>" +
                                    "</td>" +
                                "</tr> ";
                    count++;
                }
                subject = subject.Replace("{{passengerList}}", passenger);

                //thông tin chuyến bay đi
                var flyBookingDetailGo = flyBookingDetailList.FirstOrDefault(n => n.Leg == (int)CommonConstant.FlyBookingDetailType.GO);
                if (flyBookingDetailGo != null)
                {
                    subject = subject.Replace("{{isDisplayGo}}", "");
                    var airportCodeStartPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailGo.StartPoint);
                    var airportCodeEndPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailGo.EndPoint);
                    subject = subject.Replace("{{flyOrderNoGo}}", flyBookingDetailGo.BookingCode);

                    FlightSegment flightSegment = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingDetailGo.Id);
                    if (flightSegment != null)
                    {
                        subject = subject.Replace("{{handBaggageGo}}", flightSegment.HandBaggage);//hành lý chiều đi
                        subject = subject.Replace("{{allowanceBaggageGo}}", flightSegment.AllowanceBaggage);//hành lý ký gửi chiều đi
                        //get logo theo flightSegment.FlightNumber, get url ảnh
                        subject = subject.Replace("{{logoAirlineGo}}", images_url + flyBookingDetailGo.Airline.ToLower() + ".png");
                        var airline = listAirlines.FirstOrDefault(n => n.Code == flyBookingDetailGo.Airline);//get theo flyBookingDetailGo.Airline
                        subject = subject.Replace("{{flyNameGo}}", airline?.NameVi);
                        subject = subject.Replace("{{flyCodeGo}}", flightSegment.FlightNumber);
                        subject = subject.Replace("{{dayGo}}", flightSegment.StartTime != null ?
                            GetDay(flightSegment.StartTime.DayOfWeek) : "");
                        subject = subject.Replace("{{dateGo}}", flightSegment.StartTime != null ?
                            flightSegment.StartTime.ToString("dd/MM/yyyy") : "");
                        subject = subject.Replace("{{addressGoFrom}}", flyBookingDetailGo.StartPoint + "-" +
                            airportCodeStartPoint?.District_Vi);//HAN - Hà Nội
                        subject = subject.Replace("{{addressGoTo}}", flyBookingDetailGo.EndPoint + "-" +
                            airportCodeEndPoint?.District_Vi);// PQC - Phú Quốc
                        var groupclassAirline = Repository.getGroupClassAirlines(flightSegment.Class, flyBookingDetailGo.Airline, flyBookingDetailGo.GroupClass);
                        if (groupclassAirline != null)
                            subject = subject.Replace("{{flyTicketClassGo}}", groupclassAirline?.Detail_Vi);
                        else
                            subject = subject.Replace("{{flyTicketClassGo}}", flyBookingDetailGo.GroupClass);

                        subject = subject.Replace("{{timeFromGo}}", flyBookingDetailGo.StartDate.Value.ToString("HH:mm"));//10:40
                        subject = subject.Replace("{{timeToGo}}", flyBookingDetailGo.EndDate.Value.ToString("HH:mm"));//22:45
                    }
                }
                else
                {
                    subject = subject.Replace("{{displayChieuDi}}", @"style=" + "\"" + "display: none!important;" + "\"");
                    subject = subject.Replace("{{isDisplayGo}}", "display: none!important;");
                }

                //thông tin chuyến bay về
                var flyBookingDetailBack = flyBookingDetailList.FirstOrDefault(n => n.Leg != (int)CommonConstant.FlyBookingDetailType.GO);
                if (flyBookingDetailBack != null)
                {
                    subject = subject.Replace("{{isDisplayBack}}", "");
                    subject = subject.Replace("{{flyOrderNoBack}}", flyBookingDetailBack.BookingCode);
                    var airportCodeStartPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailBack.StartPoint);
                    var airportCodeEndPoint = listAirportCode.FirstOrDefault(n => n.Code == flyBookingDetailBack.EndPoint);
                    FlightSegment flightSegment = flightSegmentList.FirstOrDefault(n => n.FlyBookingId == flyBookingDetailBack.Id);
                    subject = subject.Replace("{{handBaggageBack}}", flightSegment.HandBaggage);//hành lý chiều về
                    subject = subject.Replace("{{allowanceBaggageBack}}", flightSegment.AllowanceBaggage);//hành lý ký gửi chiều về
                    subject = subject.Replace("{{flyCodeBack}}", flightSegment.FlightNumber);
                    subject = subject.Replace("{{addressBackFrom}}", flyBookingDetailBack.StartPoint + "-" +
                            airportCodeStartPoint?.District_Vi);//HAN - Hà Nội
                    subject = subject.Replace("{{addressBackTo}}", flyBookingDetailBack.EndPoint + "-" +
                        airportCodeEndPoint?.District_Vi);// PQC - Phú Quốc
                    subject = subject.Replace("{{dayBack}}", flightSegment.StartTime != null ?
                        GetDay(flightSegment.StartTime.DayOfWeek) : "");
                    subject = subject.Replace("{{dateBack}}", flightSegment.StartTime != null ?
                            flightSegment.StartTime.ToString("dd/MM/yyyy") : "");
                    subject = subject.Replace("{{timeFromBack}}", flyBookingDetailBack.StartDate.Value.ToString("HH:mm"));//10:40
                    subject = subject.Replace("{{timeToBack}}", flyBookingDetailBack.EndDate.Value.ToString("HH:mm"));//22:45

                    //get logo theo flightSegment.FlightNumber, get url ảnh
                    subject = subject.Replace("{{logoAirlineBack}}", images_url + flyBookingDetailBack.Airline.ToLower() + ".png");
                    var airline = listAirlines.FirstOrDefault(n => n.Code == flyBookingDetailBack.Airline);//get theo flyBookingDetailBack.Airline
                    subject = subject.Replace("{{flyNameBack}}", airline?.NameVi);
                    var groupclassAirline = Repository.getGroupClassAirlines(flightSegment.Class, flyBookingDetailBack.Airline, flyBookingDetailBack.GroupClass);
                    if (groupclassAirline != null)
                        subject = subject.Replace("{{flyTicketClassBack}}", groupclassAirline?.Detail_Vi);
                    else
                        subject = subject.Replace("{{flyTicketClassBack}}", flyBookingDetailBack.GroupClass);
                }
                else
                {
                    subject = subject.Replace("{{displayChieuVe}}", @"style=" + "\"" + "display: none!important;" + "\"");
                    subject = subject.Replace("{{isDisplayBack}}", "display: none!important;");
                }

                var data_VietQRBankList = ApiService.GetVietQRBankList().Result;
                var selected_bank = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("Techcombank".Trim().ToLower())) : null;
                string bank_code = "Techcombank";
                if (selected_bank != null) bank_code = selected_bank.bin;
                var result = ApiService.GetVietQRCode("19131835226016", bank_code, orderInfo.OrderNo, Convert.ToDouble(orderInfo.Amount)).Result;
                var jsonData = JObject.Parse(result);
                var status = int.Parse(jsonData["code"].ToString());
                if (status == (int)ResponseType.SUCCESS)
                {
                    var url_path = ApiService.UploadImageQRBase64(orderInfo.OrderNo, Convert.ToDouble(orderInfo.Amount).ToString(), jsonData["data"]["qrDataURL"].ToString(), "19131835226016").Result;

                    subject = subject.Replace("{{LinkQR}}", ConfigurationManager.AppSettings["IMAGE_DOMAIN"] + url_path);
                }
                //link thanh toán
                subject = subject.Replace("{{payLink}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_PAYMENT"] + flyBookingDetail.Session + "&clientId=" + orderInfo.AccountClientId);
                //link thanh toán xong
                subject = subject.Replace("{{payLinkDone}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_CHECK_PAYMENT"] + orderInfo.OrderId + "&clientId=" + orderInfo.AccountClientId);

                SendEmail(orderInfo.OrderNo, subject, contactClient?.Email, client.Email, true);
                //-- Logging
                // Telegram.pushLog("APP.CHECKOUT_SERVICE - Send Email Order :"+ orderInfo.OrderId+" - " + orderInfo.OrderNo+" - ClientID: "+orderInfo.ClientId+" . Email Reveived: "+ contactClient?.Email);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Service sendMailOrderB2C. Exception" + ex);
                return false;
            }

        }

        /// <summary>
        /// Scope: B2B
        /// Thực hiện gửi mail theo rule của mkt dựa vào order_id
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public bool sendMailOrderB2B(int order_id)
        {
            try
            {
                //2 get template mail
                string projectDirectory = Environment.CurrentDirectory;
                var template = projectDirectory + @"/EmailTemplate/B2B/MailTemplateB2B.html";
                var subject = File.ReadAllText(template);

                if (order_id == -1)
                {
                    Telegram.pushLog("Service sendMailOrderB2B. Create order Fail. order_id = " + order_id);
                    return false;
                }

                //1. get order info
                var orderInfo = Repository.getOrderDetail(order_id);

                //số tiền thanh toán
                subject = subject.Replace("{{total}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));
                //số tiền cần thanh toán
                subject = subject.Replace("{{amount}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));

                //var client = Repository.getClientDetail(orderInfo != null ? orderInfo.ClientId.Value : 0);
                ContactClient contactClient = Repository.getContactClient((int)orderInfo.ContactClientId);

                var listHotelBooking = Repository.getHotelBookings(order_id);
                var listHotelBookingRoom = new List<APP.CHECKOUT_SERVICE.ViewModel.Order.HotelBookingRooms>();
                var listHotelBookingRoomRate = new List<APP.CHECKOUT_SERVICE.ViewModel.Order.HotelBookingRoomRates>();
                string hotelBookingId = string.Empty;
                var listHotelGuest = new List<HotelGuest>();
                foreach (var item in listHotelBooking)
                {
                    hotelBookingId = item.BookingId;
                    var hotelBookingRooms = Repository.getHotelBookingRoom(item.Id);
                    foreach (var room in hotelBookingRooms)
                    {
                        listHotelBookingRoom.Add(room);
                        var hotelBookingRoomRates = Repository.getHotelBookingRoomRate(room.Id);
                        listHotelBookingRoomRate.Add(hotelBookingRoomRates.FirstOrDefault());
                    }
                    var hotelGuests = Repository.getHotelGuest(item.Id);
                    foreach (var guest in hotelGuests)
                    {
                        listHotelGuest.Add(guest);
                    }
                }

                var hotelBooking = listHotelBooking.FirstOrDefault();

                if (hotelBooking != null)
                {
                    if (listHotelBooking.FirstOrDefault(n => string.IsNullOrEmpty(n.BookingId)) != null)
                    {
                        subject = subject.Replace("{{keepTicketTime}}", "Thời gian thanh toán đến " + orderInfo.ExpriryDate.ToString("HH:mm:ss dd/MM/yyyy"));
                        subject = subject.Replace("{{keepTicketTimeText}}", " <br>Loại vé không giữ được chỗ. Vui lòng liên hệ hotline 093.6191.192 để đặt vé");
                    }
                    else
                    {
                        subject = subject.Replace("{{keepTicketTime}}", "Đang giữ phòng đến " + orderInfo.ExpriryDate.ToString("HH:mm:ss dd/MM/yyyy"));
                        subject = subject.Replace("{{keepTicketTimeText}}", "");
                    }

                    subject = subject.Replace("{{checkinTime}}", ((DateTime)hotelBooking.CheckinTime).ToString("HH:mm"));
                    subject = subject.Replace("{{checkoutTime}}", ((DateTime)hotelBooking.CheckoutTime).ToString("HH:mm"));
                    subject = subject.Replace("{{hotelName}}", hotelBooking.HotelName);
                    subject = subject.Replace("{{hotelAddress}}", hotelBooking.Address);
                    subject = subject.Replace("{{hotelBookingGo}}", orderInfo.OrderNo);
                    subject = subject.Replace("{{hotelImage}}", hotelBooking.ImageThumb);
                    subject = subject.Replace("{{hotelEmail}}", hotelBooking.Email);
                    subject = subject.Replace("{{hotelPhone}}", hotelBooking.Telephone);
                    subject = subject.Replace("{{receiveRoom}}", GetDay(hotelBooking.ArrivalDate.DayOfWeek) + ", ngày " +
                        hotelBooking.ArrivalDate.ToString("dd") + " tháng " + hotelBooking.ArrivalDate.ToString("MM") + " năm "
                        + hotelBooking.ArrivalDate.ToString("yyyy"));
                    subject = subject.Replace("{{returnRoom}}", GetDay(hotelBooking.DepartureDate.DayOfWeek) + ", ngày " +
                        hotelBooking.DepartureDate.ToString("dd") + " tháng " + hotelBooking.DepartureDate.ToString("MM") + " năm "
                        + hotelBooking.DepartureDate.ToString("yyyy"));
                    subject = subject.Replace("{{urlMyOrder}}", "");

                }

                var hotelRooms = string.Empty;
                foreach (var item in listHotelBookingRoom)
                {
                    string numberOfRoom = string.Empty;
                    if (item.NumberOfAdult != null && item.NumberOfAdult > 0)
                    {
                        numberOfRoom += item.NumberOfAdult + " người lớn ";
                    }
                    if (item.NumberOfChild != null && item.NumberOfChild > 0)
                    {
                        numberOfRoom += item.NumberOfChild + " trẻ em ";
                    }
                    if (item.NumberOfInfant != null && item.NumberOfInfant > 0)
                    {
                        numberOfRoom += item.NumberOfInfant + " trẻ sơ sinh ";
                    }
                    string packageIncludes = string.Empty;
                    if (!string.IsNullOrEmpty(item.PackageIncludes))
                    {
                        var listPackageInclude = item.PackageIncludes.Split(",");
                        foreach (var package in listPackageInclude)
                        {
                            packageIncludes += " <li>" + package + "</li>";
                        }
                    }
                    var hotelRoomRate = listHotelBookingRoomRate.FirstOrDefault(n => n.HotelBookingRoomId == item.Id);

                    hotelRooms += @" <tr>" +
                                "<td> " +
                                    "<strong>" + item.RoomTypeName + "</strong> " +
                                "</td>" +
                                "<td>" + numberOfRoom + "</td>" +
                                "<td>" +
                                   " <ul style=\"padding - left: 15px; \">" +
                                     " <li> Tên gói: " + hotelRoomRate?.RatePlanCode + "</li>" +
                                        packageIncludes +
                                      "</ul> " +
                                 " </td> " +
                              "</tr>";
                }
                subject = subject.Replace("{{hotelRooms}}", hotelRooms);

                var hotelGuest = string.Empty;
                var count = 1;
                var countRoom = 1;
                foreach (var item in listHotelBookingRoom)
                {
                    var listGuest = listHotelGuest.Where(n => n.HotelBookingRoomsID == item.Id && n.HotelBookingId == item.HotelBookingId).ToList();
                    foreach (var guest in listGuest)
                    {
                        hotelGuest += @" <tr>" +
                               "<td>" + count + "</td>" +
                               "<td> Phòng " + countRoom + "</td>" +
                               "<td> " +
                                   "<strong>" + item.RoomTypeName + "</strong> " +
                               "</td>" +
                               "<td>" + guest.Name + "</td>" +
                               "<td></td>" +
                             "</tr>";
                        count++;
                    }
                    countRoom++;
                }
                subject = subject.Replace("{{hotelGuests}}", hotelGuest);

                //3. fill data to template
                //thông tin order
                subject = subject.Replace("{{orderNo}}", orderInfo.OrderNo);
                subject = subject.Replace("{{orderDate}}", orderInfo.CreateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"));
                //thông tin sale
                var User = Repository.getUserDetail((long)orderInfo.SalerId);

                //thông tin khách hàng
                subject = subject.Replace("{{customerName}}", contactClient?.Name);
                subject = subject.Replace("{{gender}}", "");
                subject = subject.Replace("{{phone}}", contactClient?.Mobile);
                subject = subject.Replace("{{email}}", contactClient?.Email);


                //link thanh toán
                subject = subject.Replace("{{payLink}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_PAYMENT"] + hotelBooking?.BookingId + "&clientId=" + orderInfo.AccountClientId);
                //link thanh toán xong
                subject = subject.Replace("{{payLinkDone}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_CHECK_PAYMENT"] + orderInfo.OrderId + "&clientId=" + orderInfo.AccountClientId);

                SendEmail(orderInfo.OrderNo, subject, contactClient?.Email, User.Email, true);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Service sendMailOrderB2B. Exception: " + ex);
                return false;
            }

        }

        public bool sendMailVinWonder(int order_id)
        {
            try
            {
                //2 get template mail
                string projectDirectory = Environment.CurrentDirectory;
                var template = projectDirectory + @"/EmailTemplate/VinWonder/MailTemplateVinWonder.html";
                var subject = File.ReadAllText(template);

                if (order_id == -1)
                {
                    Telegram.pushLog("Service sendMailVinWonder. Create order Fail. order_id = " + order_id);
                    return false;
                }

                //1. get order info
                var orderInfo = Repository.getOrderDetail(order_id);
                //số tiền thanh toán
                subject = subject.Replace("{{total}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));
                //số tiền cần thanh toán
                subject = subject.Replace("{{amount}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));

                //var client = Repository.getClientDetail(orderInfo != null ? orderInfo.ClientId.Value : 0);
                ContactClient contactClient = Repository.getContactClient((int)orderInfo.ContactClientId);

                var listVinWonderBooking = Repository.getVinWonderBookings(order_id);
                var listVinWonderBookingTicket = new List<VinWonderBookingTicket>();
                //var listVinWonderBookingTicketCustomer = new List<VinWonderBookingTicketCustomer>();
                foreach (var item in listVinWonderBooking)
                {
                    var vinWonderBookingTicket = Repository.getVinWonderBookingTickets(item.Id);
                    foreach (var ticket in vinWonderBookingTicket)
                    {
                        listVinWonderBookingTicket.Add(ticket);
                    }
                }

                var vinWonderBooking = listVinWonderBooking.FirstOrDefault();

                if (vinWonderBooking != null)
                {
                    subject = subject.Replace("{{keepTicketTime}}", "Đang giữ vé đến " + orderInfo.ExpriryDate.ToString("HH:mm:ss dd/MM/yyyy"));
                    subject = subject.Replace("{{keepTicketTimeText}}", "");
                }

                var vinWonderTickets = string.Empty;
                foreach (var item in listVinWonderBookingTicket)
                {
                    var strTicket = string.Empty;
                    if (item.Adt > 0)
                    {
                        strTicket += "<li>" + item.Adt + " x Vé VinWonders - Người lớn x " + item.Quantity + "</li> ";
                    }
                    if (item.Child > 0)
                    {
                        strTicket += "<li>" + item.Child + " x Vé VinWonders - Trẻ em x " + item.Quantity + "</li> ";
                    }
                    if (item.Old > 0)
                    {
                        strTicket += "<li>" + item.Old + " x Vé VinWonders - Người cao tuổi x " + item.Quantity + "</li> ";
                    }
                    vinWonderTickets += @" <tr>" +
                                "<td> " +
                                  "<p style='font-weight: bolder;'>" + vinWonderBooking.SiteName + "</p>" +
                                  "<p >" + item.Name + "</p>" +
                                "</td>" +
                                "<td>" + "<ul>" + strTicket + "</ul>" + "</td>" +
                                "<td>" +
                                    (item.DateUsed != null ? item.DateUsed.Value.ToString("dd/MM/yyyy") : "") +
                                 " </td> " +
                              "</tr>";
                }
                subject = subject.Replace("{{vinWonderTickets}}", vinWonderTickets);

                //3. fill data to template
                //thông tin order
                subject = subject.Replace("{{orderNo}}", orderInfo.OrderNo);
                subject = subject.Replace("{{orderDate}}", orderInfo.CreateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"));

                //thông tin khách hàng
                subject = subject.Replace("{{customerName}}", contactClient?.Name);
                subject = subject.Replace("{{gender}}", "");
                subject = subject.Replace("{{phone}}", contactClient?.Mobile);
                subject = subject.Replace("{{email}}", contactClient?.Email);

                var data_VietQRBankList = ApiService.GetVietQRBankList().Result;
                var selected_bank = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("Techcombank".Trim().ToLower())) : null;
                string bank_code = "Techcombank";
                if (selected_bank != null) bank_code = selected_bank.bin;
                var result = ApiService.GetVietQRCode("19131835226016", bank_code, orderInfo.OrderNo, Convert.ToDouble(orderInfo.Amount)).Result;
                var jsonData = JObject.Parse(result);
                var status = int.Parse(jsonData["code"].ToString());
                if (status == (int)ResponseType.SUCCESS)
                {
                    var url_path = ApiService.UploadImageQRBase64(orderInfo.OrderNo, Convert.ToDouble(orderInfo.Amount).ToString(), jsonData["data"]["qrDataURL"].ToString(), "19131835226016").Result;

                    subject = subject.Replace("{{LinkQR}}", ConfigurationManager.AppSettings["IMAGE_DOMAIN"] + url_path);
                }
                //link thanh toán
                subject = subject.Replace("{{payLink}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_PAYMENT_VIN_WONDER"] + vinWonderBooking?.AdavigoBookingId + "&clientId=" + orderInfo.AccountClientId);
                //link thanh toán xong
                subject = subject.Replace("{{payLinkDone}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_CHECK_PAYMENT_VIN_WONDER"] + orderInfo.OrderId + "&clientId=" + orderInfo.AccountClientId);

                SendEmail(orderInfo.OrderNo, subject, contactClient?.Email);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Service sendMailVinWonder. Exception: " + ex);
                return false;
            }
        }

        public bool sendMailVinWonderReceiveMoney(int order_id)
        {
            try
            {
                //2 get template mail
                string projectDirectory = Environment.CurrentDirectory;
                var template = projectDirectory + @"/EmailTemplate/VinWonder/MailTemplateVinWonder.html";
                var subject = File.ReadAllText(template);

                if (order_id == -1)
                {
                    Telegram.pushLog("Service sendMailVinWonder. Create order Fail. order_id = " + order_id);
                    return false;
                }

                //1. get order info
                var orderInfo = Repository.getOrderDetail(order_id);
                //số tiền thanh toán
                subject = subject.Replace("{{total}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));
                //số tiền cần thanh toán
                subject = subject.Replace("{{amount}}", orderInfo.Amount?.ToString("N0").Replace(',', '.'));

                //var client = Repository.getClientDetail(orderInfo != null ? orderInfo.ClientId.Value : 0);
                ContactClient contactClient = Repository.getContactClient((int)orderInfo.ContactClientId);

                var listVinWonderBooking = Repository.getVinWonderBookings(order_id);
                var listVinWonderBookingTicket = new List<VinWonderBookingTicket>();
                //var listVinWonderBookingTicketCustomer = new List<VinWonderBookingTicketCustomer>();
                foreach (var item in listVinWonderBooking)
                {
                    var vinWonderBookingTicket = Repository.getVinWonderBookingTickets(item.Id);
                    foreach (var ticket in vinWonderBookingTicket)
                    {
                        listVinWonderBookingTicket.Add(ticket);
                    }
                }

                var vinWonderBooking = listVinWonderBooking.FirstOrDefault();

                if (vinWonderBooking != null)
                {
                    subject = subject.Replace("{{keepTicketTime}}", "Đang giữ vé đến " + orderInfo.ExpriryDate.ToString("HH:mm:ss dd/MM/yyyy"));
                    subject = subject.Replace("{{keepTicketTimeText}}", "");
                }

                var vinWonderTickets = string.Empty;
                foreach (var item in listVinWonderBookingTicket)
                {
                    var strTicket = string.Empty;
                    if (item.Adt > 0)
                    {
                        strTicket += "<li>" + item.Adt + " x Vé VinWonders - Người lớn" + "</li> ";
                    }
                    if (item.Child > 0)
                    {
                        strTicket += "<li>" + item.Child + " x Vé VinWonders - Trẻ em" + "</li> ";
                    }
                    if (item.Old > 0)
                    {
                        strTicket += "<li>" + item.Old + " x Vé VinWonders - Trẻ sơ sinh" + "</li> ";
                    }
                    vinWonderTickets += @" <tr>" +
                                "<td> " +
                                  "<p style='font-weight: bolder;'>" + vinWonderBooking.SiteName + "</p>" +
                                  "<p >" + item.Name + "</p>" +
                                "</td>" +
                                "<td>" + "<ul>" + strTicket + "</ul>" + "</td>" +
                                "<td>" +
                                    (item.DateUsed != null ? item.DateUsed.Value.ToString("dd/MM/yyyy") : "") +
                                 " </td> " +
                              "</tr>";
                }
                subject = subject.Replace("{{vinWonderTickets}}", vinWonderTickets);

                //3. fill data to template
                //thông tin order
                subject = subject.Replace("{{orderNo}}", orderInfo.OrderNo);
                subject = subject.Replace("{{orderDate}}", orderInfo.CreateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"));

                //thông tin khách hàng
                subject = subject.Replace("{{customerName}}", contactClient?.Name);
                subject = subject.Replace("{{gender}}", "");
                subject = subject.Replace("{{phone}}", contactClient?.Mobile);
                subject = subject.Replace("{{email}}", contactClient?.Email);


                //link thanh toán
                subject = subject.Replace("{{payLink}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_PAYMENT_VIN_WONDER"] + orderInfo.AccountClientId + "&bookingId=" + vinWonderBooking?.Id);
                //link thanh toán xong
                subject = subject.Replace("{{payLinkDone}}", ConfigurationManager.AppSettings["domain_qc_b2c"] +
                    ConfigurationManager.AppSettings["MAIL_URL_CHECK_PAYMENT_VIN_WONDER"] + orderInfo.OrderId + "&clientId=" + orderInfo.AccountClientId);

                SendEmail(orderInfo.OrderNo, subject, contactClient?.Email);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Service sendMailVinWonderReceiveMoney. Exception: " + ex);
                return false;
            }
        }
        public bool sendMailHotelBooking(int orderid)
        {
            try
            {
                var _KeyEncodeParam = "AH0fVJAdavigofnZQFg5Qaqr";
                var order = Repository.getOrderDetail(orderid);
                var hotel = Repository.getHotelBookings(orderid);
                //thông tin sale
                var User = Repository.getUserDetail((long)order.SalerId);
                string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var template = workingDirectory + @"/EmailTemplate/OrderHotelMailTemplate.html";
                string body = File.ReadAllText(template);
                var client = Repository.getContactClient((int)order.ContactClientId);

                if (order != null)
                {
                    if (order.VoucherId != null)
                    {

                        body = body.Replace("{{VoucherCode}}", order.VoucherCode);
                        body = body.Replace("{{AmountVoucher}}", "- " + (order.Discount != null ? Convert.ToDouble(order.Discount).ToString("N0") : "0"));
                    }
                    else
                    {
                        body = body.Replace("{{VoucherCode}}", "");
                        body = body.Replace("{{AmountVoucher}}", "0");
                    }
                    body = body.Replace("{{OrderNo}}", order.OrderNo);
                    body = body.Replace("{{CreatedDate}}", ((DateTime)order.CreateTime).ToString("dd/MM/yyyy"));
                    body = body.Replace("{{totalAmount}}", Convert.ToDouble(order.Amount).ToString("N0"));


                    if (client != null)
                    {
                        body = body.Replace("{{ClientName}}", client.Name != null ? client.Name : "");
                        body = body.Replace("{{Phone}}", client.Mobile != null ? client.Mobile : "");
                        body = body.Replace("{{Email}}", client.Email != null ? client.Email : "");
                    }
                    else
                    {
                        body = body.Replace("{{ClientName}}", "");
                        body = body.Replace("{{Phone}}", "");
                        body = body.Replace("{{Email}}", "");

                    }

                }
                else
                {
                    body = body.Replace("{{OrderNo}}", "");
                    body = body.Replace("{{CreatedDate}}", "");
                    body = body.Replace("{{totalAmount}}", "0");


                }
                if (hotel != null)
                {
                    var DetailHotelBooking = Repository.GetDetailHotelBookingByID(hotel[0].Id);
                    var HotelBookingRooms = Repository.getHotelBookingRoom(hotel[0].Id);
                    if (HotelBookingRooms != null)
                    {
                        body = body.Replace("{{totalAmount3}}", HotelBookingRooms.Sum(s => s.TotalAmount).ToString("N0"));
                    }
                    else
                    {
                        body = body.Replace("{{totalAmount3}}", "0");
                    }
                    if (DetailHotelBooking != null)
                    {

                        body = body.Replace("{{TotalRooms}}", DetailHotelBooking[0].TotalRooms.ToString("N0"));

                        string table = "";
                        
                        var rooms = Repository.GetHotelBookingRoomsOptionalByBookingId(hotel[0].Id);
                        var packages = Repository.GetHotelBookingRoomRatesOptionalByBookingId(hotel[0].Id);
                        if (rooms != null && rooms.Count > 0)
                            foreach (var item in rooms)
                            {
                                string RatePlanCode = String.Empty;
                                string date = String.Empty;
                                double Nights = 0;
                                string TotalAmount = String.Empty;
                                string Saleprice = String.Empty;
                                string Goi = String.Empty;
                                string TgSD = String.Empty;
                                string GiaN = String.Empty;
                                string SDem = String.Empty;
                                string SP = String.Empty;
                                string TTien = String.Empty;
                                double NumberOfRooms = 0;
                               
                                var package_by_room_id = packages.Where(x => x.HotelBookingRoomOptionalId == item.Id);
                                if (package_by_room_id != null && package_by_room_id.Count() > 0)
                                {
                                    string table_row = "";
                                    string table_row2 = "";
                                    foreach (var p in package_by_room_id)
                                    {
                                        double operator_price = 0;
                                        if (p.Price != null) operator_price = Math.Round(((double)p.SalePrice * (double)p.Nights * (double)item.NumberOfRooms), 0);
                                        if (operator_price <= 0) operator_price = p.SalePrice != null ? (double)p.SalePrice : 0;

                                        RatePlanCode = p.RatePlanCode;
                                        date = (p.StartDate == null ? "" : ((DateTime)p.StartDate).ToString("dd/MM/yyyy")) + " - " + (p.EndDate == null ? "" : ((DateTime)p.EndDate).ToString("dd/MM/yyyy"));
                                        Saleprice = p.Price == null ? ((double)p.SalePrice).ToString("N0") : ((double)p.SalePrice).ToString("N0"); 
                                        Nights = (int)p.Nights;
                                        TotalAmount = operator_price.ToString("N0");
                                        NumberOfRooms = item.NumberOfRooms == null ? 1 : (double)item.NumberOfRooms;
                                        Goi = "<div style='text-align: center;'>" + RatePlanCode + "</div>";
                                        TgSD = "<div style='text-align: center;'>" + date + "</div>";
                                        GiaN = "<div style='text-align: center;'>" + Saleprice + " đ</div>";
                                        SDem = "<div style='text-align: center;'>" + Nights + "</div>";
                                        SP = "<div style='text-align: center;'>" + NumberOfRooms + "</div>";
                                        TTien = "<div style='text-align: center;'>" + TotalAmount + " đ</div>";
                                        table_row += "<tr>" +
                                                             "<td style='border: 1px solid #999;text-align: center;'>" + Goi + "</td>" +
                                                             "<td style='border: 1px solid #999;text-align: center;'>" + TgSD + "</td>" +
                                                             "<td style='border: 1px solid #999;text-align: center;'>" + GiaN + "</td>" +
                                                             "<td style='border: 1px solid #999;text-align: center;'>" + SDem + "</td>" +
                                                             "<td style='border: 1px solid #999;text-align: center;'>" + TTien + "</td>"
                                                              + "</tr>";
                                    }
                                    var rowspan = package_by_room_id.Count() + 1;
                                    table += "<tr>" +
                                                "<th rowspan="+ rowspan + " style='border: 1px solid #999;font-weight: normal;'>" + item.RoomTypeName + "</th>" +
                                                "<td style='padding: 0px !important;'></td>" +
                                                "<td style='padding: 0px !important;'></td>" +
                                                "<td style='padding: 0px !important;'></td>" +
                                                "<td style='padding: 0px !important;;'></td>" +
                                                 "<th rowspan=" + rowspan + " style='border: 1px solid #999;font-weight: normal;'>" + SP + "</th>" +
                                                 "<td style='padding: 0px !important;;'></td>" +
                                            "</tr>" +
                                              table_row
                                                              ;
                                }
                                

                            }
                        //foreach (var item in DetailHotelBooking)
                        //{
                        //    table += "<tr>" +
                        //        "<td>" +
                        //           " <strong>" + item.RoomTypeName + "</strong>" +
                        //                "<div style = 'color: #698096;' >" + item.TotalRooms + " phòng</ div >" +
                        //                "<div style = 'color: #698096;' >" + ((item.DepartureDate - item.ArrivalDate).TotalDays) + " Đêm</ div >" +

                        //              " </td>" +
                        //               "<td>" + item.NumberOfAdult + " người, " + item.NumberOfChild + " trẻ em, " + item.NumberOfInfant + " trẻ sơ sinh</ td >" +
                        //                 " <td> " +

                        //                     "</td>" +

                        //                 "</tr>";
                        //}

                        string dataTable = "<table cellspacing='0' cellpadding='0' width='100%'> <tbody>" +

                                 "<tr>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;width: 153px;'>Ngày nhận phòng:</td>" +
                                        "<th style='border: 1px solid #999; padding: 5px;text-align: center;font-weight: normal;'colspan='2'>" + Convert.ToDateTime(hotel[0].ArrivalDate).ToString("dd/MM/yyyy")+"</th>" +
                                        "<td style='border: 1px solid #999; padding: 5px; font-weight: bold;'colspan='2'>Ngày trả phòng:</td>" +
                                        "<th style='border: 1px solid #999; padding: 5px;text-align: center;font-weight: normal;'colspan='2'>" + Convert.ToDateTime(hotel[0].DepartureDate).ToString("dd/MM/yyyy")+"</th>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style = 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số lượng phòng:</ td >" +

                                        "<td style = 'border: 1px solid #999; padding: 5px;text-align: center;'colspan='2'>" + DetailHotelBooking[0].TotalRooms + "</td> " +

                                        "<th rowspan = '2' style = 'border: 1px solid #999; padding: 5px; font-weight: bold;'colspan='2' > Số lượng khách(NL/ TE / EB):</th>" +

                                        "<th rowspan = '2' style = 'border: 1px solid #999; padding: 5px;text-align: center;font-weight: normal;'colspan='2' >" + DetailHotelBooking[0].NumberOfAdult+ "/"+ DetailHotelBooking[0].NumberOfChild + "/" + DetailHotelBooking[0].NumberOfInfant +"</th>" +

                                      "</tr > " +
                                      "<tr>"+

                                           "<td style = 'border: 1px solid #999; padding: 5px; font-weight: bold;' > Số đêm </ td >" +

                                            "<td style = 'border: 1px solid #999; padding: 5px;text-align: center;' colspan='2'>" + (Convert.ToDateTime(hotel[0].DepartureDate) - Convert.ToDateTime(hotel[0].ArrivalDate)).TotalDays.ToString("N0") + "</td>" +
                                       "</tr >" +
                                      "<tr>" +
                                         "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Hạng phòng</th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Gói phòng</th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thời gian sử dụng</th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Giá </th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số đêm</th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Số phòng</th>" +
                                          "<th style='border: 1px solid #999; padding: 2px; text-align: center;'>Thành tiền</th>" +
                                      "</tr> " +

                                    table +
                                   "</tbody></table>";

                        body = body.Replace("{{DataTable}}", dataTable);
                    }
                    else
                    {
                        body = body.Replace("{{DataTable}}", "");

                        body = body.Replace("{{TotalRooms}}", "0");
                        body = body.Replace("{{TotalDays}}", "0");
                    }
                    //body = body.Replace("{{CheckinTime}}", Convert.ToDateTime(hotel[0].CheckinTime).ToString("dd/MM/yyyy"));
                    //body = body.Replace("{{CheckoutTime}}", Convert.ToDateTime(hotel[0].CheckoutTime).ToString("dd/MM/yyyy"));
                    body = body.Replace("{{TotalDays}}", (Convert.ToDateTime(hotel[0].DepartureDate) - Convert.ToDateTime(hotel[0].ArrivalDate)).TotalDays.ToString("N0"));
                    body = body.Replace("{{HotelName}}", hotel[0].HotelName);
                    body = body.Replace("{{Address}}", hotel[0].Address);
                    body = body.Replace("{{ImageThumb}}", hotel[0].ImageThumb);
                    if(hotel[0].Note !=null && hotel[0].Note != "")
                    {
                        var note_hote = "<div style='font-size:18px;margin-bottom:15px'><strong>Ghi chú</strong></div>" +
                            "<table class='table-border' width='100%'>" +
                            "<tr>" +
                            "<td style='border: 1px solid #999;padding:5px'>" + hotel[0].Note + "</td></tr></table>";
                        body = body.Replace("{{Note}}", note_hote);
                    }
                    else
                    {
                        body = body.Replace("{{Note}}", "");
                    }
                    
                }
                else
                {
                    body = body.Replace("{{CheckinTime}}", "");
                    body = body.Replace("{{CheckoutTime}}", "");
                    body = body.Replace("{{HotelName}}", "");
                    body = body.Replace("{{Address}}", "");
                    body = body.Replace("{{ImageThumb}}", "");
                }

                var payment_token = CommonHelper.Encode(JsonConvert.SerializeObject(new HotelPaymentModel
                {
                    hotelID = hotel[0].Id.ToString(),
                    hotelName = hotel[0].HotelName,
                    arrivalDate = hotel[0].ArrivalDate,
                    departureDate = hotel[0].DepartureDate,
                    numberOfRoom = hotel[0].NumberOfRoom,
                    numberOfAdult = hotel[0].NumberOfAdult,
                    numberOfChild = hotel[0].NumberOfChild,
                    numberOfInfant = hotel[0].NumberOfInfant,
                    totalMoney = (decimal)order.Amount,// model.rooms.Sum(x => x.amount),
                    bookingID = orderid.ToString(),
                    orderID = orderid.ToString()
                }), _KeyEncodeParam);
                var listHotelBooking = Repository.getHotelBookings(orderid);
                var listHotelGuest = new List<HotelGuest>();
                var listHotelBookingRoom = new List<APP.CHECKOUT_SERVICE.ViewModel.Order.HotelBookingRooms>();
                var listHotelBookingRoomRate = new List<APP.CHECKOUT_SERVICE.ViewModel.Order.HotelBookingRoomRates>();
                string hotelBookingId = string.Empty;
                foreach (var item in listHotelBooking)
                {
                    hotelBookingId = item.BookingId;
                    var hotelBookingRooms = Repository.getHotelBookingRoom(item.Id);
                    foreach (var room in hotelBookingRooms)
                    {
                        listHotelBookingRoom.Add(room);
                        var hotelBookingRoomRates = Repository.getHotelBookingRoomRate(room.Id);
                        listHotelBookingRoomRate.Add(hotelBookingRoomRates.FirstOrDefault());
                    }
                    var hotelGuests = Repository.getHotelGuest(item.Id);
                    foreach (var guest in hotelGuests)
                    {
                        listHotelGuest.Add(guest);
                    }
                }
                var hotelGuest = string.Empty;
              
                var count = 1;
                var countRoom = 1;
                var listHotelBookingRoom2 = listHotelBookingRoom.GroupBy(s => new { s.RoomTypeName,s.Id}).Select(i => i.First()).ToList();
                foreach (var item in listHotelBookingRoom2)
                {
                    var hotelGuest_row = string.Empty;
                    var listGuest = listHotelGuest.Where(n => n.HotelBookingRoomsID == item.Id && n.HotelBookingId == item.HotelBookingId).ToList();
                    
                    foreach (var guest in listGuest)
                    {
                        hotelGuest_row +=" <tr>"+
                               "<td style='border: 1px solid #999;padding:5px'>" + guest.Name + "</td>" +
                               "<td style='border: 1px solid #999;padding:5px'></td>" +
                              "</tr>";
                        count++;
                    }
                    var count_row = listGuest.Count + 1;
                    hotelGuest += @"<tr>" +
                              "<th rowspan="+ count_row + " style='border: 1px solid #999;padding:5px;font-weight:normal'>" + countRoom + "</th>" +
                              "<th rowspan=" + count_row + " style='border: 1px solid #999;padding:5px;font-weight:normal'> Phòng " + countRoom + "</th>" +
                              "<th rowspan=" + count_row + " style='border: 1px solid #999;padding:5px;font-weight:normal'> " +
                                   item.RoomTypeName +
                              "</th>" +
                              "<td  style='padding: 0px !important;'></td>" +
                              "<td  style='padding: 0px !important;'></td>" +
                            "</tr>"+
                            hotelGuest_row;
                    countRoom++;
                }
                var data_VietQRBankList =  ApiService.GetVietQRBankList().Result;
                var selected_bank = data_VietQRBankList.Count > 0 ? data_VietQRBankList.FirstOrDefault(x => x.shortName.Trim().ToLower().Contains("Techcombank".Trim().ToLower())) : null;
                string bank_code = "Techcombank";
                if (selected_bank != null) bank_code = selected_bank.bin;
                var result = ApiService.GetVietQRCode("19131835226016", bank_code, order.OrderNo, Convert.ToDouble(order.Amount)).Result;
                var jsonData = JObject.Parse(result);
                var status = int.Parse(jsonData["code"].ToString());
                if (status == (int)ResponseType.SUCCESS)
                {
                    var url_path =  ApiService.UploadImageQRBase64(order.OrderNo, Convert.ToDouble(order.Amount).ToString(), jsonData["data"]["qrDataURL"].ToString(), "19131835226016").Result;

                    body = body.Replace("{{LinkQR}}", ConfigurationManager.AppSettings["IMAGE_DOMAIN"] + url_path);
                }
            
                body = body.Replace("{{hotelGuests}}", hotelGuest);               
                body = body.Replace("{{Link}}", ConfigurationManager.AppSettings["domain_qc_b2b"] + "/hotel/payment?booking=" + payment_token);
                body = body.Replace("{{totalAmount2}}", "0");
                body = body.Replace("{{AmountPayment}}", "0");
                SendEmail(order.OrderNo, body, client?.Email, User.Email, true);
                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Service sendMailOrderB2B. Exception: " + ex);
                return false;
            }

        }

        private void SendEmail(string orderNo, string subject, string email, string clientEmail = "", bool isB2C = false)
        {
            //4. send my using smtp 
            var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["STMP_HOST"],
                Port = int.Parse(ConfigurationManager.AppSettings["STMP_PORT"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["STMP_USERNAME"],
                ConfigurationManager.AppSettings["STMP_APP_PASSWORD"]),
                Timeout = 10000,
            };

            var message = new MailMessage ()
            {
                IsBodyHtml = true,
                From = new MailAddress(ConfigurationManager.AppSettings["STMP_FROM_MAIL"], ConfigurationManager.AppSettings["STMP_DISPLAYNAME"]),
                Subject = "THÔNG TIN ĐƠN HÀNG " + orderNo,
                Body = subject,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8,
            };
            if (isB2C)
                message.CC.Add(new MailAddress(clientEmail));
            message.To.Add(email);
            if (ConfigurationManager.AppSettings["Environment"] != null && ConfigurationManager.AppSettings["Environment"] == "QC")
            {
                message.To.Add("truongthuy0401@gmail.com");
                message.To.Add("anhhieuk51@gmail.com");
                //message.To.Add("thang.nguyenvan1@vti.com.vn");
            }
            smtp.Send(message);
        }

        public string GetDay(DayOfWeek day)
        {
            var dayStr = String.Empty;
            if (day == DayOfWeek.Monday)
            {
                dayStr = "Thứ 2";
            }
            if (day == DayOfWeek.Tuesday)
            {
                dayStr = "Thứ 3";
            }
            if (day == DayOfWeek.Wednesday)
            {
                dayStr = "Thứ 4";
            }
            if (day == DayOfWeek.Thursday)
            {
                dayStr = "Thứ 5";
            }
            if (day == DayOfWeek.Friday)
            {
                dayStr = "Thứ 6";
            }
            if (day == DayOfWeek.Saturday)
            {
                dayStr = "Thứ 7";
            }
            if (day == DayOfWeek.Sunday)
            {
                dayStr = "Chủ nhật";
            }
            return dayStr;
        }

    }
}

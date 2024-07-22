using APP.CHECKOUT_SERVICE.Behaviors;
using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using Newtonsoft.Json;
using System;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Engines
{
    public class CheckOutFactory : ICheckOutFactory
    {
        private readonly IOrderFlyBookingService fly_service;
        private readonly IOrderHotelRentsService hotel_service;
        private readonly IMailService mail_service;
        private readonly ILogActivityService log_activity_service;
        private readonly INotifyService notify_service;
        private readonly IOrderTourBookingService _tourBookingService;
        private readonly IOrderVinWonderService _vinWonderService;

        public CheckOutFactory(IOrderFlyBookingService _order_service, IMailService _mail_service, ILogActivityService _log_activity_service, INotifyService _notify_service, IOrderHotelRentsService _hotel_service, IOrderVinWonderService vinWonderService,
            IOrderTourBookingService tourBookingService)
        {
            fly_service = _order_service;
            hotel_service = _hotel_service;
            mail_service = _mail_service;
            log_activity_service = _log_activity_service;
            notify_service = _notify_service;
            _vinWonderService = vinWonderService;
            _tourBookingService = tourBookingService;
        }
        /// <summary>
        /// order_info: là thông tin đơn hàng được push về từ frontend
        /// </summary>
        /// <param name="order_info"></param>
        public async void DoSomeRealWork(OrderEntities order_info)
        {
            try
            {
                Console.WriteLine("Input: " + JsonConvert.SerializeObject(order_info));
                //0. Detect dịch vụ
                switch (order_info.service_type)
                {
                    case (int)ServicesType.FlyingTicket:
                        {
                            switch (order_info.event_status)
                            {
                                case 0:
                                    {

                                        //1.1. Tạo đơn hàng + Tạo chi tiết đơn hàng  (MINH)
                                        FlyBookingOrderSummit order_summit = fly_service.flyBookingOrderFromMessage(order_info).Result;
                                        if (order_summit == null) break;
                                        int order_id = fly_service.createOrder(order_summit, order_info);
                                        // nếu tạo đơn ko thành công, không thực hiện gửi mail (lỗi đã báo trong hàm tạo đơn):
                                        if (order_id < 0) break;

                                        //1.2. Send mail (THANG)
                                        Console.WriteLine("Send mail with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailOrderB2C(order_id);
                                        Console.WriteLine("Send mail with order id = " + order_id + ". Result: " + is_send_mail);

                                        //1.3. Lưu log activity vào MONGO DB thông qua api core (HIEU)
                                        bool is_save_log = log_activity_service.logCreateOrder(order_info);
                                        //4. Notify tới  Telegram khi đơn đc khởi tạo thành công (HIEU): Nội dung thông báo lấy từ Bình
                                        notify_service.pushNotiCreateOrder(order_id);
                                        var push=notify_service.SendNotifyCMS(order_id).Result;
                                    } break;
                                case 1:
                                    {
                                        // Case 2: Khi khách hàng muốn thanh toán lại đơn hàng bằng hình thức khác:
                                        //2.1: Cập nhật hình thức thanh toán đơn hàng
                                        OrderWithOldPaymentType updated_order = fly_service.UpdatePaymentTypeWithOrderID(order_info).Result;
                                        if (updated_order == null) break;
                                        //2.2. Lưu log activity vào MONGO DB thông qua api core 
                                        bool is_save_log = log_activity_service.logUpdateOrderPaymentType(order_info);
                                        //2.3. Notify tới Telegram thông báo đơn hàng thay đổi hình thức thanh toán
                                        notify_service.pushNotiUpdateOrderPaymentType(updated_order);
                                        
                                    }
                                    break;
                                case 2:
                                    {

                                        int order_id = Convert.ToInt32(order_info.order_id);
                                        Console.WriteLine("Send mail b2c with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailOrderB2C(order_id);
                                        Console.WriteLine("Send mail b2c with order id = " + order_id + ". Result: " + is_send_mail);
                                    }
                                    break;
                            }
   
                        }
                        break;
                    case (int)ServicesType.OthersHotelRent:
                    case (int)ServicesType.VINHotelRent:
                        {
                            switch (order_info.event_status)
                            {
                                case 0:
                                    {

                                        //1.1. Tạo đơn hàng + Tạo chi tiết đơn hàng  (MINH)
                                        HotelRentOrderSummit order_summit = hotel_service.HotelRentSummitFromMessage(order_info).Result;
                                        if (order_summit == null) break;
                                        int order_id = hotel_service.createOrder(order_summit, order_info);
                                        // nếu tạo đơn ko thành công, không thực hiện gửi mail (lỗi đã báo trong hàm tạo đơn):
                                        if (order_id < 0) break;

                                        //1.2. Send mail : CAll API Send Email
                                      //  Console.WriteLine("Send mail with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailHotelBooking(order_id);
                                        Console.WriteLine("Send mail with order id = " + order_id + ". Result: " + is_send_mail);

                                        //1.3. Lưu log activity vào MONGO DB thông qua api core (HIEU)
                                        bool is_save_log = log_activity_service.logCreateOrder(order_info);
                                        //4. Notify tới  Telegram khi đơn đc khởi tạo thành công (HIEU): Nội dung thông báo lấy từ Bình
                                        notify_service.pushNotiCreateOrder(order_id);
                                        var push = notify_service.SendNotifyCMS(order_id).Result;

                                    }
                                    break;
                                case 1:
                                    {
                                        // Case 2: Khi khách hàng muốn thanh toán lại đơn hàng bằng hình thức khác:
                                        //2.1: Cập nhật hình thức thanh toán đơn hàng
                                        OrderWithOldPaymentType updated_order = hotel_service.UpdatePaymentTypeWithOrderID(order_info).Result;
                                        if (updated_order == null) break;
                                        //2.2. Lưu log activity vào MONGO DB thông qua api core 
                                        bool is_save_log = log_activity_service.logUpdateOrderPaymentType(order_info);
                                        //2.3. Notify tới Telegram thông báo đơn hàng thay đổi hình thức thanh toán
                                        notify_service.pushNotiUpdateOrderPaymentType(updated_order);
                                    }
                                    break;
                                case 2:
                                    {
                                        // Send mail : CAll API Send Email
                                        int order_id = Convert.ToInt32(order_info.order_id);
                                       // Console.WriteLine("Send mail b2b with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailOrderB2B(order_id);
                                        Console.WriteLine("Send mail b2b with order id = " + order_id + ". Result: " + is_send_mail);
                                    }
                                    break;
                            }

                        }
                        break;
                    case (int)ServicesType.VinWonder:
                        {
                            switch (order_info.event_status)
                            {
                                case 0:
                                    {

                                        //1.1. Tạo đơn hàng + Tạo chi tiết đơn hàng  (MINH)
                                        VinWonderTicketSummit order_summit = _vinWonderService.VinWonderTicketFromMessage(order_info).Result;
                                        if (order_summit == null) break;
                                        int order_id = _vinWonderService.createOrder(order_summit, order_info);
                                        // nếu tạo đơn ko thành công, không thực hiện gửi mail (lỗi đã báo trong hàm tạo đơn):
                                        if (order_id < 0) break;

                                        //1.2. Send mail : CAll API Send Email
                                       // Console.WriteLine("Send mail with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailVinWonder(order_id);
                                        Console.WriteLine("Send mail with order id = " + order_id + ". Result: " + is_send_mail);


                                        //1.3. Lưu log activity vào MONGO DB thông qua api core (HIEU)
                                        bool is_save_log = log_activity_service.logCreateOrder(order_info);
                                        //4. Notify tới  Telegram khi đơn đc khởi tạo thành công (HIEU): Nội dung thông báo lấy từ Bình
                                        notify_service.pushNotiCreateOrder(order_id);
                                        var push = notify_service.SendNotifyCMS(order_id).Result;

                                    }
                                    break;
                                case 1:
                                    {
                                        // Case 2: Khi khách hàng muốn thanh toán lại đơn hàng bằng hình thức khác:
                                        //2.1: Cập nhật hình thức thanh toán đơn hàng
                                        OrderWithOldPaymentType updated_order = hotel_service.UpdatePaymentTypeWithOrderID(order_info).Result;
                                        if (updated_order == null) break;
                                        //2.2. Lưu log activity vào MONGO DB thông qua api core 
                                        bool is_save_log = log_activity_service.logUpdateOrderPaymentType(order_info);
                                        //2.3. Notify tới Telegram thông báo đơn hàng thay đổi hình thức thanh toán
                                        notify_service.pushNotiUpdateOrderPaymentType(updated_order);
                                    }
                                    break;
                                case 2:
                                    {
                                        /*
                                        // Send mail : CAll API Send Email
                                        int order_id = Convert.ToInt32(order_info.order_id);
                                        Console.WriteLine("Send mail b2b with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailOrderB2B(order_id);
                                        Console.WriteLine("Send mail b2b with order id = " + order_id + ". Result: " + is_send_mail);*/
                                    }
                                    break;
                            }

                        }
                        break;
                    case (int)ServicesType.Tourist:
                        {
                          
                            switch (order_info.event_status)
                            {
                                case 0:
                                    {
                                        //1.1. Tạo đơn hàng + Tạo chi tiết đơn hàng  (MINH)
                                        TourSummit order_summit = _tourBookingService.TourFromMessage(order_info).Result;
                                        if (order_summit == null) break;
                                        int order_id = _tourBookingService.createOrder(order_summit, order_info);
                                        // nếu tạo đơn ko thành công, không thực hiện gửi mail (lỗi đã báo trong hàm tạo đơn):
                                        if (order_id < 0) break;



                                        //1.3. Lưu log activity vào MONGO DB thông qua api core (HIEU)
                                        bool is_save_log = log_activity_service.logCreateOrder(order_info);
                                        //4. Notify tới  Telegram khi đơn đc khởi tạo thành công (HIEU): Nội dung thông báo lấy từ Bình
                                        notify_service.pushNotiCreateOrder(order_id);
                                        var push = notify_service.SendNotifyCMS(order_id).Result;

                                    }
                                    break;
                                case 1:
                                    {
                                        if (order_info.order_id > 0)
                                        {
                                            var exists_order = await _tourBookingService.GetOrderInfo(order_info.order_id);
                                            if(exists_order!=null && exists_order.OrderId > 0 && (exists_order.Amount==null || exists_order.Amount<=0))
                                            {

                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else if(order_info.booking_id !=null && order_info.booking_id.Trim()!="" && order_info.order_id <= 0)
                                        {

                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //1.1. Tạo đơn hàng + Tạo chi tiết đơn hàng  (MINH)
                                        TourSummit order_summit = _tourBookingService.TourFromMessage(order_info).Result;
                                        if (order_summit == null) break;
                                        int order_id = _tourBookingService.createOrder(order_summit, order_info);
                                        // nếu tạo đơn ko thành công, không thực hiện gửi mail (lỗi đã báo trong hàm tạo đơn):
                                        if (order_id < 0) break;



                                        //1.3. Lưu log activity vào MONGO DB thông qua api core (HIEU)
                                        bool is_save_log = log_activity_service.logCreateOrder(order_info);
                                        //4. Notify tới  Telegram khi đơn đc khởi tạo thành công (HIEU): Nội dung thông báo lấy từ Bình
                                        notify_service.pushNotiCreateOrder(order_id);
                                        var push = notify_service.SendNotifyCMS(order_id).Result;


                                        //// Case 2: Khi khách hàng muốn thanh toán lại đơn hàng bằng hình thức khác:
                                        ////2.1: Cập nhật hình thức thanh toán đơn hàng
                                        //OrderWithOldPaymentType updated_order = _tourBookingService.UpdatePaymentTypeWithOrderID(order_info).Result;
                                        //if (updated_order == null) break;
                                        ////2.2. Lưu log activity vào MONGO DB thông qua api core 
                                        //bool is_save_log = log_activity_service.logUpdateOrderPaymentType(order_info);
                                        ////2.3. Notify tới Telegram thông báo đơn hàng thay đổi hình thức thanh toán
                                        //notify_service.pushNotiUpdateOrderPaymentType(updated_order);
                                    }
                                    break;
                                case 2:
                                    {
                                        /*
                                        // Send mail : CAll API Send Email
                                        int order_id = Convert.ToInt32(order_info.order_id);
                                        Console.WriteLine("Send mail b2b with order id = " + order_id);
                                        bool is_send_mail = mail_service.sendMailOrderB2B(order_id);
                                        Console.WriteLine("Send mail b2b with order id = " + order_id + ". Result: " + is_send_mail);*/
                                    }
                                    break;
                            }
                        }
                        break;
                }
                
              
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

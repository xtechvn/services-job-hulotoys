using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.Model;
using APP.CHECKOUT_SERVICE.ViewModel.Log;
using APP.CHECKOUT_SERVICE.ViewModel.Mail;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Engines.Notify
{
    public class NotifyService : INotifyService
    {
        private static string api_insert_log = ConfigurationManager.AppSettings["domain_api_core"];
        private static string key_encrypt = ConfigurationManager.AppSettings["key_encrypt_log"];
        public bool pushNotiCreateOrder(int order_id)
        {

            try
            {
                var orderItems = Repository.getOrderDetail(order_id);
                var client = Repository.getClientDetail((long)orderItems.ClientId);
                ContactClient contactClient = Repository.getContactClient((int)orderItems.ContactClientId);
                HttpClient httpClient = new HttpClient();
                string payment_type_string = "CHUYỂN KHOẢN TRỰC TIẾP";
                switch (orderItems.PaymentType)
                {
                    case (int)PaymentType.ATM:
                        {
                            payment_type_string = "THẺ ATM/ TÀI KHOẢN NGÂN HÀNG";
                            break;
                        }

                    case (int)PaymentType.GIU_CHO:
                        {
                            payment_type_string = "GIỮ CHỖ";
                            break;
                        }
                    case (int)PaymentType.KY_QUY:
                        {
                            payment_type_string = "THANH TOÁN BẰNG KÝ QUỸ";
                            break;
                        }
                    case (int)PaymentType.QR_PAY:
                        {
                            payment_type_string = "THANH TOÁN QR PAY";
                            break;
                        }
                    case (int)PaymentType.TAI_VAN_PHONG:
                        {
                            payment_type_string = "THANH TOÁN TẠI VĂN PHÒNG";
                            break;
                        }
                    case (int)PaymentType.VISA_MASTER_CARD:
                        {
                            payment_type_string = "THẺ VISA / MASTERCARD";
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }
                string log = "Đơn hàng " + orderItems.OrderNo + " đã tạo mới thành công qua hình thức " + payment_type_string.ToUpper() + "."
                  + "\nBởi tài khoản ID: " + orderItems.AccountClientId + ". Email người đặt đơn: " + client.Email
                  + "\nGửi đến người nhận vé tại Email: " + contactClient.Email
                  + "\nSố tiền : " + ((double)orderItems.Amount).ToString("N0")
                  + "\nTạo lúc: " + ((DateTime)(orderItems.CreateTime)).ToString("dd/MM/yyyy HH:mm");

                log = "Đơn hàng " + orderItems.OrderNo + " đã tạo mới thành công qua hình thức " + payment_type_string.ToUpper() + "."
                                  + "\nBởi tài khoản ID: " + orderItems.AccountClientId
                                  + "\nEmail người đặt đơn: " + client.Email
                                  + "\nGửi đến người nhận tại Email: " + contactClient.Email
                                  + "\nSố tiền : " + ((double)orderItems.Amount).ToString("N0")
                                  + "\nTạo lúc: " + ((DateTime)(orderItems.CreateTime)).ToString("dd/MM/yyyy HH:mm");
                SystemLog logmodel = new SystemLog();
                logmodel.Log = log;
                logmodel.KeyID = orderItems.OrderNo;
                logmodel.SourceID = (int)SystemLogSourceID.BOT_CHECKOUT;
                logmodel.Type = SystemLogTypeID.WARNING;
                var logcontent = JsonConvert.SerializeObject(logmodel);
                string token = CommonHelper.Encode(logcontent, key_encrypt);
                //push sang api
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token",token)
                });
                var resultApi = httpClient.PostAsync(api_insert_log + "/api/LogManager/insert-app-log.json", content).Result;

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("logCreateOrder LogActivityService " + ex.ToString());
                return false;
            }
        }

        public bool pushNotiUpdateOrderPaymentType(OrderWithOldPaymentType orderItems)
        {

            try
            {
                var client = Repository.getClientDetail((long)orderItems.ClientId);
                HttpClient httpClient = new HttpClient();
                string payment_type_string = "CHUYỂN KHOẢN TRỰC TIẾP";
                string payment_type_old = "CHUYỂN KHOẢN TRỰC TIẾP";
                switch (orderItems.PaymentType)
                {
                    case (int)PaymentType.ATM:
                        {
                            payment_type_string = "THẺ ATM/ TÀI KHOẢN NGÂN HÀNG";
                            break;
                        }

                    case (int)PaymentType.GIU_CHO:
                        {
                            payment_type_string = "GIỮ CHỖ";
                            break;
                        }
                    case (int)PaymentType.KY_QUY:
                        {
                            payment_type_string = "THANH TOÁN BẰNG KÝ QUỸ";
                            break;
                        }
                    case (int)PaymentType.QR_PAY:
                        {
                            payment_type_string = "THANH TOÁN QR PAY";
                            break;
                        }
                    case (int)PaymentType.TAI_VAN_PHONG:
                        {
                            payment_type_string = "THANH TOÁN TẠI VĂN PHÒNG";
                            break;
                        }
                    case (int)PaymentType.VISA_MASTER_CARD:
                        {
                            payment_type_string = "THẺ VISA / MASTERCARD";
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }
                switch (orderItems.old_payment_type)
                {
                    case (int)PaymentType.ATM:
                        {
                            payment_type_old = "THẺ ATM/ TÀI KHOẢN NGÂN HÀNG";
                            break;
                        }

                    case (int)PaymentType.GIU_CHO:
                        {
                            payment_type_old = "GIỮ CHỖ";
                            break;
                        }
                    case (int)PaymentType.KY_QUY:
                        {
                            payment_type_old = "THANH TOÁN BẰNG KÝ QUỸ";
                            break;
                        }
                    case (int)PaymentType.QR_PAY:
                        {
                            payment_type_old = "THANH TOÁN QR PAY";
                            break;
                        }
                    case (int)PaymentType.TAI_VAN_PHONG:
                        {
                            payment_type_old = "THANH TOÁN TẠI VĂN PHÒNG";
                            break;
                        }
                    case (int)PaymentType.VISA_MASTER_CARD:
                        {
                            payment_type_old = "THẺ VISA / MASTERCARD";
                            break;
                        }
                    default:
                        {
                            break;
                        }

                }
                string log = "Đơn hàng " + orderItems.OrderNo + " đã chuyển đổi hình thức thanh toán từ " + payment_type_old.ToUpper() + " thành " + payment_type_string.ToUpper() + " ." +
                    "\n Vào lúc: " + ((DateTime)(orderItems.CreateTime)).ToString("dd/MM/yyyy HH:mm");
                SystemLog logmodel = new SystemLog();
                logmodel.Log = log;
                logmodel.KeyID = orderItems.OrderNo;
                logmodel.SourceID = (int)SystemLogSourceID.BOT_CHECKOUT;
                logmodel.Type = SystemLogTypeID.WARNING;
                var logcontent = JsonConvert.SerializeObject(logmodel);
                string token = CommonHelper.Encode(logcontent, key_encrypt);
                //push sang api
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token",token)
                });
                var resultApi = httpClient.PostAsync(api_insert_log + "/api/LogManager/insert-app-log.json", content).Result;
                Console.WriteLine("logCreateOrder LogActivityService " + log);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("logCreateOrder LogActivityService " + ex.ToString());
                Console.WriteLine("logCreateOrder LogActivityService " + ex.ToString());
                return false;
            }
        }
        public async Task<int> SendNotifyCMS(long order_id)
        {
            try
            {
                var orderItems = Repository.getOrderDetail((int)order_id);

                HttpClient httpClient = new HttpClient();
                var j_param = new Dictionary<string, object>
                {
                    {"user_name_send",  "Bot Adavigo"}, //tên người gửi
                    {"user_id_send", Convert.ToInt64(ConfigurationManager.AppSettings["Created_By_BotID"])}, //id người gửi
                    {"code", orderItems.OrderNo}, // mã đối tượng gửi
                    {"service_code", ""}, // mã đối tượng gửi
                    {"link_redirect", ConfigurationManager.AppSettings["CMS_ORDER_DETAIL"].Replace("{orderid}",order_id.ToString())}, // Link mà khi người dùng click vào detail item notify sẽ chuyển sang đó
                    {"module_type",(int)ModuleType.DON_HANG}, // loại module thực thi luồng notify. Ví dụ: Đơn hàng, khách hàng.......
                    {"action_type", (int)ActionType.TAO_MOI}, // action thực hiện. Ví dụ: Duyệt, tạo mới, từ chối....
                    {"role_type", ""} // quyền mà sẽ gửi tới
                };
                var data_product = JsonConvert.SerializeObject(j_param);

                var token = CommonHelper.Encode(data_product, ConfigurationManager.AppSettings["key_encrypt"]);
                var request = new FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("token",token)
                });
                var url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_NOTIFY"];
                var response = await httpClient.PostAsync(url, request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    var result = resultContent_2["status"].ToString();
                }
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }

                return 1;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("SendNotifyCMS LogActivityService " + ex.ToString());
                return 1;
            }
        }
    }
}

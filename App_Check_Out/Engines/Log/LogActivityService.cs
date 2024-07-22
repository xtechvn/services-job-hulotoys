using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.ViewModel.Log;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Engines.Log
{
    public class LogActivityService : ILogActivityService
    {
        private static string api_insert_log = ConfigurationManager.AppSettings["domain_api_core"];
        private static string key_encrypt = ConfigurationManager.AppSettings["key_encrypt_log"];
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        /// <summary>
        /// Log thông tin đơn hàng khi bên Frontend gửi sang.
        /// push sang api log 
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        /// 

        public bool logCreateOrder(OrderEntities order_info)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var log = JsonConvert.SerializeObject(order_info);
                SystemLog logmodel = new SystemLog();
                logmodel.Log = log;
                logmodel.KeyID = order_info.order_no;
                logmodel.SourceID = (int)SystemLogSourceID.APP;
                logmodel.Type = SystemLogTypeID.BOOKING;
                var logcontent = JsonConvert.SerializeObject(logmodel);
                string token = CommonHelper.Encode(logcontent, key_encrypt);
                //push sang api
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token",token)
                });
                var resultApi = httpClient.PostAsync(api_insert_log + "/api/LogManager/insert-app-log.json", content);

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("logCreateOrder LogActivityService " + ex.ToString());
                return false;
            }

        }
        public bool logUpdateOrderPaymentType(OrderEntities order_info)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var log = "Update Order Payment with:\n "+ JsonConvert.SerializeObject(order_info);
                SystemLog logmodel = new SystemLog();
                logmodel.Log = log;
                logmodel.KeyID = order_info.order_no;
                logmodel.SourceID = (int)SystemLogSourceID.APP;
                logmodel.Type = SystemLogTypeID.BOOKING;
                var logcontent = JsonConvert.SerializeObject(logmodel);
                string token = CommonHelper.Encode(logcontent, key_encrypt);
                //push sang api
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token",token)
                });
                var resultApi = httpClient.PostAsync(api_insert_log + "/api/LogManager/insert-app-log.json", content);
                Console.WriteLine("logUpdateOrderPaymentType- Done - Result:true ");

                return true;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("logCreateOrder LogActivityService " + ex.ToString());
                return false;
            }

        }
    }
}

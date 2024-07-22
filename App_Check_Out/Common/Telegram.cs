using APP.CHECKOUT_SERVICE.ViewModel.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Common
{
    public static class Telegram
    {
        static string path = Directory.GetCurrentDirectory() + @"\log";
        static string api_url = ConfigurationManager.AppSettings["domain_api_core"] + ConfigurationManager.AppSettings["API_PushLog"];
        static string key_ecrypt = ConfigurationManager.AppSettings["key_encrypt_log"];
        /// <summary>
        /// Minh tích hợp call api push log tele
        /// Thông tin noti cho các rule đơn hàng
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task pushNotify(string msg)
        {          
            try
            {
                var report_text = msg;
                var httpClient_2 = new HttpClient();
                var url = api_url;
                var input = new SystemLog()
                {
                    SourceID = (int)SystemLogSourceID.APP,
                    Type = SystemLogTypeID.ACTIVITY,
                    Log = "[APP.CHECKOUT_SERVICE] " + report_text
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), key_ecrypt);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                    });
                var result_2 = await httpClient_2.PostAsync(url, content_2);
                var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(result_2.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                
                ErrorWriter.WriteLog(path, "[pushNotify error] msg = " + msg + " msg err:  " + ex.ToString());
            
            }
        }

        /// <summary>
        ///Thông tin noti Xử lý các lỗi trong app
        /// </summary>
        /// <param name="msg"></param>
        public static async Task pushLog(string msg)
        {
            try
            {
                var report_text = msg;
                var httpClient_2 = new HttpClient();
                var url = api_url;
                var input = new SystemLog()
                {
                    SourceID = (int)SystemLogSourceID.APP,
                    Type = SystemLogTypeID.ERROR,
                    Log = "[" + ConfigurationManager.AppSettings["Environment"] + "]"+ "[APP.CHECKOUT_SERVICE] " + report_text
                };
                var token = CommonHelper.Encode(JsonConvert.SerializeObject(input), key_ecrypt);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                       new KeyValuePair<string, string>("token", token),
                    });
                var result_2 = await httpClient_2.PostAsync(url, content_2);
                var resultContent_2 = Newtonsoft.Json.Linq.JObject.Parse(result_2.Content.ReadAsStringAsync().Result);
                ErrorWriter.WriteLog(path, "[APP.CHECKOUT_SERVICE] activity: " + msg);

            }
            catch (Exception ex)
            {
                ErrorWriter.WriteLog(path, "[pushNotify error] msg = " + msg + " msg err:  " + ex.ToString());

            }
        }


    }
}

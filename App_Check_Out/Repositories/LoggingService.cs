using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;

namespace APP.READ_MESSAGES.Libraries
{
    public  class LoggingService : ILoggingService
    {
        private readonly IConfiguration _configuration;
        public string token = "5321912147:AAFhcJ9DolwPWL74WbMjOOyP6-0G7w88PWY";
        public string group = "-620666227";
        public string env = "Product";
        public string name = "APP_CHECKOUT";

        public LoggingService(IConfiguration configuration) {

            _configuration = configuration;
            token = configuration["BotSetting:bot_token"];
            group = configuration["BotSetting:bot_group_id"];
            env = configuration["BotSetting:environment"];
            name = configuration["BotSetting:Name"];

        }

    
        public  void LoggingAppOutput( string message, bool file_out = false,bool tele_out=false)
        {
            string log = "[" + name + "] - [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]: " + message + ""; 
            if (file_out)
            {
                WriteAppLog(log);
            }
            if (tele_out)
            {
                InsertLogTelegramDirect(log);
            }
        }
        private  bool WriteAppLog(string log)
        {
            try
            {
                string app_path = Directory.GetCurrentDirectory().Replace(@"\bin\Debug\netcoreapp3.1", "") + @"\log";
                if (!Directory.Exists(app_path))
                {
                    Directory.CreateDirectory(app_path);
                }
                string file_name = "general" + DateTime.Now.ToString("yyMMdd") + ".list";
                string path_excuted = app_path + @"\" +file_name;
                if (!File.Exists(path_excuted))
                {
                    File.Create(path_excuted);
                }
                File.AppendAllText(path_excuted, log);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private int InsertLogTelegramDirect(string message)
        {
            var rs = 1;
            try
            {
                TelegramBotClient alertMsgBot = new TelegramBotClient(token);
                var rs_push = alertMsgBot.SendTextMessageAsync(group, message).Result;
            }
            catch (Exception)
            {
                rs = -1;
            }
            return rs;
        }
       
    }
}

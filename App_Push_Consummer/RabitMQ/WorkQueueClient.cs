
using App_Push_Consummer.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Configuration;
using System.Reflection;
using System.Text;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Models.Queue;

namespace App_Push_Consummer.RabitMQ
{
    public class WorkQueueClient
    {
        private readonly QueueSettingViewModel queue_setting;
        private readonly ConnectionFactory factory;
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        public WorkQueueClient()
        {
            queue_setting = new QueueSettingViewModel()
            {
                host = ConfigurationManager.AppSettings["QUEUE_HOST"],
                port = Convert.ToInt32(ConfigurationManager.AppSettings["QUEUE_PORT"]),
                v_host = ConfigurationManager.AppSettings["QUEUE_V_HOST_SYNC"],
                username = ConfigurationManager.AppSettings["QUEUE_USERNAME"],
                password = ConfigurationManager.AppSettings["QUEUE_PASSWORD"],
                queue_Name = ConfigurationManager.AppSettings["QUEUE_SYNC_ES"],
            };
            factory = new ConnectionFactory()
            {
                HostName = queue_setting.host,
                UserName = queue_setting.username,
                Password = queue_setting.password,
                VirtualHost = queue_setting.v_host,
                Port = Protocols.DefaultProtocol.DefaultPort
            };
        }
        public bool SyncES(int id,string store_procedure,string index_es,short project_id)
        {
            try
            {
                var j_param = new Dictionary<string, object>
                              {
                              { "store_name", store_procedure },
                              { "index_es", index_es },
                              {"project_type", project_id },
                              {"id" , id }

                              };
                var _data_push = JsonConvert.SerializeObject(j_param);
                // Push message vào queue
                var response_queue = InsertQueueSimpleDurable(_data_push, ConfigurationManager.AppSettings["QUEUE_SYNC_ES"]);
                ErrorWriter.InsertLogTelegram(tele_token, tele_group_id, "WorkQueueClient - SyncES[" + id + "][" + store_procedure + "] [" + index_es + "][" + project_id + "]: "+ response_queue.ToString());

                return true;
            }
            catch(Exception ex)
            {

            }return false;
        }
        public bool InsertQueueSimple(string message, string queueName)
        {            
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                    return true;

                }
                catch (Exception ex)
                {
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "WorkQueueClient - InsertQueueSimple[" + message + "][" + queueName + "]: " + ex.ToString());
                    return false;
                }
            }
        }
        public bool InsertQueueSimpleDurable(string message, string queueName)
        {

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: body);
                    return true;

                }
                catch (Exception ex)
                {
                    string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "WorkQueueClient - InsertQueueSimpleDurable[" + message + "][" + queueName + "]: " + ex.ToString());
                    return false;
                }
            }
        }
    }
}

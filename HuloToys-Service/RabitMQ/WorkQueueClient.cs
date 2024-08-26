using HuloToys_Service.Models.Queue;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using RabbitMQ.Client;
using System.Reflection;
using System.Text;

namespace HuloToys_Service.RabitMQ
{
    public class WorkQueueClient
    {
        private readonly IConfiguration configuration;
        public WorkQueueClient(IConfiguration _configuration)
        {
            configuration = _configuration;            
        }
        public bool InsertQueueSimple(QueueSettingViewModel queue_setting,string message, string queueName)
        {            
            var factory = new ConnectionFactory()
            {
                HostName = queue_setting.host,
                UserName = queue_setting.username,
                Password = queue_setting.password,
                VirtualHost = queue_setting.v_host,
                Port = Protocols.DefaultProtocol.DefaultPort
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                     durable: false,
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
                    string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                    LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                    return false;
                }
            }
        }
        public bool InsertQueueSimpleDurable(QueueSettingViewModel queue_setting, string message, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = queue_setting.host,
                UserName = queue_setting.username,
                Password = queue_setting.password,
                VirtualHost = queue_setting.v_host,
                Port = Protocols.DefaultProtocol.DefaultPort
            };
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
                    string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                    LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                    return false;
                }
            }
        }
    }
}

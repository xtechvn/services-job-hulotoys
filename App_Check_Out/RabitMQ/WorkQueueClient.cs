using APP.READ_MESSAGES.Libraries;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Reflection;
using System.Text;
using WEB.CMS.Models.Queue;

namespace HuloToys_Service.RabitMQ
{
    public class WorkQueueClient
    {
        private readonly IConfiguration configuration;
        private readonly QueueSettingViewModel queue_setting;
        private readonly ConnectionFactory factory;
        private readonly ILoggingService logging_service;

        public WorkQueueClient(IConfiguration _configuration, ILoggingService _logging_service)
        {
            configuration = _configuration;
            queue_setting = new QueueSettingViewModel()
            {
                host = _configuration["Queue:Host"],
                port = Convert.ToInt32(_configuration["Queue:Port"]),
                v_host = _configuration["Queue:V_Host"],
                username = _configuration["Queue:Username"],
                password = _configuration["Queue:Password"],
            };
            factory = new ConnectionFactory()
            {
                HostName = queue_setting.host,
                UserName = queue_setting.username,
                Password = queue_setting.password,
                VirtualHost = queue_setting.v_host,
                Port = Protocols.DefaultProtocol.DefaultPort
            };
            logging_service=_logging_service;
        }
        public bool SyncES(long id, string store_procedure, string index_es, short project_id)
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
                var response_queue = InsertQueueSimple(_data_push, configuration["Queue:QueueNameSyncES"]);
                logging_service.LoggingAppOutput("WorkQueueClient - SyncES["+ id + "]["+ store_procedure + "] ["+ index_es + "]["+ project_id + "]: " + response_queue.ToString(), true, true);

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public bool InsertQueueSimple(string message, string queueName)
        {            
            
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
                    logging_service.LoggingAppOutput("WorkQueueClient - InsertQueueSimple[" + message + "][" + queueName + "]: " + ex.ToString(), true, true);

                    return false;
                }
            }
        }
        public bool InsertQueueSimpleDurable( string message, string queueName)
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
                    
                    return false;
                }
            }
        }
    }
}

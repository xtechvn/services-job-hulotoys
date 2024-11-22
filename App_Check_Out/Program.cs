using APP.READ_MESSAGES.Libraries;
using APP_CHECKOUT.Interfaces;
using APP_CHECKOUT.Repositories;
using APP_CHECKOUT.Models.Models.Queue;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;



ServiceCollection service_collection = new ServiceCollection();
service_collection.AddSingleton<IMainServices, MainServices>();
service_collection.AddSingleton<ILoggingService, LoggingService>();
var service_provider = service_collection.BuildServiceProvider();
var _configuration = service_provider.GetService<IConfiguration>();
var main_service = service_provider.GetService<IMainServices>();
var log_service = service_provider.GetService<ILoggingService>();
try
{
   
    var factory = new ConnectionFactory()
    {
        HostName = _configuration["Queue:Host"],
        Port = Convert.ToInt32(_configuration["Queue:Port"]),
        VirtualHost = _configuration["Queue:V_Host"],
        UserName = _configuration["Queue:Username"],
        Password = _configuration["Queue:Password"],
    };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(queue: _configuration["Queue:QueueName"],
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        Console.WriteLine("Service Waiting: " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var request = JsonConvert.DeserializeObject<CheckoutQueueModel>(message);
                    await main_service.Excute(request);
                }
                catch { }
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                string err = "Program: " + ex.ToString();
                Console.WriteLine(err);
                log_service.LoggingAppOutput(err, true,true);

            }
        };

        channel.BasicConsume(queue: _configuration["Queue:QueueName"], autoAck: false, consumer: consumer);

        Console.ReadLine();

    }
}
catch (Exception ex)
{
    string err = "Main: " + ex.ToString();
    Console.WriteLine(err);
    log_service.LoggingAppOutput(err, true, true);
}

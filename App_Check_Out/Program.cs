using APP.CHECKOUT_SERVICE.Behaviors;
using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Engines;
using APP.CHECKOUT_SERVICE.Engines.Log;
using APP.CHECKOUT_SERVICE.Engines.Mail;
using APP.CHECKOUT_SERVICE.Engines.Notify;
using APP.CHECKOUT_SERVICE.Engines.Order;
using APP.CHECKOUT_SERVICE.Interfaces;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Text;

namespace APP.CHECKOUT_SERVICE
{
    /// <summary>
    /// App: Xử lý các tiến trình cho đơn hàng của các loại dịch vụ ADAVIGO
    /// </summary>
    class Program
    {
        private static string queue_checkout_order = ConfigurationManager.AppSettings["queue_name"];
        public static string QUEUE_HOST = ConfigurationManager.AppSettings["QUEUE_HOST"];
        public static string QUEUE_V_HOST = ConfigurationManager.AppSettings["QUEUE_V_HOST"];
        public static string QUEUE_USERNAME = ConfigurationManager.AppSettings["QUEUE_USERNAME"];
        public static string QUEUE_PASSWORD = ConfigurationManager.AppSettings["QUEUE_PASSWORD"];
        public static string QUEUE_PORT = ConfigurationManager.AppSettings["QUEUE_PORT"];
        public static string QUEUE_KEY_API = ConfigurationManager.AppSettings["QUEUE_KEY_API"];
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        static void Main(string[] args)
        {
            try
            {
                //MailService mailService = new MailService();
                //mailService.sendMailVinWonder(12224);

                #region READ QUEUE
                var factory = new ConnectionFactory()
               {
                   HostName = QUEUE_HOST,
                   UserName = QUEUE_USERNAME,
                   Password = QUEUE_PASSWORD,
                   VirtualHost = QUEUE_V_HOST,
                   Port = Protocols.DefaultProtocol.DefaultPort
               };
               using (var connection = factory.CreateConnection())
               using (var channel = connection.CreateModel())
               {
                   try
                   {
                       channel.QueueDeclare(queue: queue_checkout_order,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                       channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                       Console.WriteLine(" [*] Waiting for messages.");

                       var consumer = new EventingBasicConsumer(channel);
                       consumer.Received += (sender, ea) =>
                       {
                           try
                           {

                               var body = ea.Body.ToArray();
                               var message = Encoding.UTF8.GetString(body);

                               var serviceProvider = new ServiceCollection();
                               serviceProvider.AddSingleton<IOrderFlyBookingService, OrderFlyBookingService>();
                               serviceProvider.AddSingleton<IOrderHotelRentsService, OrderHotelRentsService>();
                               serviceProvider.AddSingleton<INotifyService, NotifyService>();
                               serviceProvider.AddSingleton<IMailService, MailService>();
                               serviceProvider.AddSingleton<ILogActivityService, LogActivityService>();
                               serviceProvider.AddSingleton<ICheckOutFactory, CheckOutFactory>();
                               serviceProvider.AddSingleton<IOrderVinWonderService, OrderVinWonderService>();
                               serviceProvider.AddSingleton<IOrderTourBookingService, OrderTourBookingService>();
                               var Service_Provider = serviceProvider.BuildServiceProvider();

                               var checkout = Service_Provider.GetService<ICheckOutFactory>();
                               try
                               {
                                   var model = JsonConvert.DeserializeObject<OrderEntities>(message);
                                   checkout.DoSomeRealWork(model);

                               }
                               catch (Exception ex)
                               {
                                   Console.WriteLine("error queue: "+ queue_checkout_order + " - data input: "+ message+"\n Error: " + ex.ToString());
                                   Telegram.pushLog("error queue: " + queue_checkout_order + " - data input: " + message + "\n Error: " + ex.ToString());
                               }

                               channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine("error queue: " + ex.ToString());
                              Telegram.pushLog("error queue = " + ex.ToString());
                           }
                       };

                       channel.BasicConsume(queue: queue_checkout_order, autoAck: false, consumer: consumer);

                       Console.ReadLine();

                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine(ex.ToString());
                       Telegram.pushLog("error queue = " + ex.ToString());                        
                   }
               }
                #endregion

            }
            catch (Exception ex)
            {
                Telegram.pushLog("Error job CHECKOUNT_SERVICE = " + queue_checkout_order + "-- error: " + ex.ToString());
                Console.WriteLine(" [x] Received message: {0}", ex.ToString());
            }
        }
       
    }
}

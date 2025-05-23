﻿using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Engines.Address;
using App_Push_Consummer.Engines.ProductRaiting;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Comments;
using App_Push_Consummer.Model.Order;
using App_Push_Consummer.Model.Queue;
using App_Push_Consummer.RabitMQ;
using HuloToys_Service.Models;
using Newtonsoft.Json;
using System.Configuration;
using Utilities.Contants;

namespace App_Push_Consummer.Engines
{
    public class Factory : IFactory
    {
        public static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];

        private readonly IAddressBusiness address_business;
        private readonly IAccountClientBusiness accountclient_business;
        private readonly ICommentsBusiness comments_business;
        private readonly IProductRaitingService productRaitingService;
        private readonly IOrderBusiness orderBusiness;
        private readonly WorkQueueClient workQueueClient;

        public Factory(IAddressBusiness _address_business, IAccountClientBusiness _accountclient_business, ICommentsBusiness _comments_business,
            IProductRaitingService _productRaitingService, IOrderBusiness _orderBusiness)
        {
            address_business = _address_business;
            accountclient_business = _accountclient_business;
            comments_business = _comments_business;
            productRaitingService = _productRaitingService;
            orderBusiness = _orderBusiness;
            workQueueClient = new WorkQueueClient();
        }

        public async void DoSomeRealWork(string data_queue)
        {
            try
            {
                var queue_info = JsonConvert.DeserializeObject<QueueModel>(data_queue);
                switch (queue_info.type)
                {
                    case QueueType.ADD_ADDRESS:
                        {
                            var address_model = JsonConvert.DeserializeObject<AddressModel>(queue_info.data_push);
                            var address_id = await address_business.saveAddressClient(address_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin địa chỉ thất bại");
                            }
                            ErrorWriter.InsertLogTelegram(tele_token, tele_group_id, "App_Push_Consummer - ADD_ADDRESS with [" + data_queue + "] success. Id="+address_id);

                            workQueueClient.SyncES(address_id, "SP_GetAddressClient", "hulotoys_sp_getaddressclient", Convert.ToInt16(ProjectType.HULOTOYS));

                            break;
                        }
                    case QueueType.UPDATE_ADDRESS:
                        {
                            var address_model = JsonConvert.DeserializeObject<AddressModel>(queue_info.data_push);
                            var address_id = await address_business.updateAddressClient(address_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Cập nhật thông tin địa chỉ thất bại");
                            }
                            ErrorWriter.InsertLogTelegram(tele_token, tele_group_id, "App_Push_Consummer - UPDATE_ADDRESS with [" + data_queue + "] success. Id=" + address_id);

                            workQueueClient.SyncES(address_id, "SP_GetAddressClient", "hulotoys_sp_getaddressclient", Convert.ToInt16(ProjectType.HULOTOYS));

                            break;
                        }
                    case QueueType.ADD_USER:
                        {
                            var accountclient_model = JsonConvert.DeserializeObject<AccountClientModel>(queue_info.data_push);
                            var address_id = await accountclient_business.saveAccountClient(accountclient_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin đăng ký người dùng thất bại");
                            }

                            break;
                        }
                    case QueueType.UPDATE_USER:
                        {
                            var accountclient_model = JsonConvert.DeserializeObject<AccountClientModel>(queue_info.data_push);
                            var address_id = await accountclient_business.updateAccountClient(accountclient_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin xác nhận quên mật khẩu thất bại");
                            }
                            break;
                        }
                    case QueueType.ADD_COMMENT:
                        {
                            var comments_model = JsonConvert.DeserializeObject<CommentsModel>(queue_info.data_push);
                            var comments_id = await comments_business.saveComments(comments_model);
                            if (comments_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin comment thất bại");
                            }
                            break;
                        }
                    case QueueType.ADD_RECEIVER_INFO_EMAIL:
                        {
                            var comments_model = JsonConvert.DeserializeObject<CommentsModel>(queue_info.data_push);
                            var comments_id = await comments_business.saveReceiverInfoEmail(comments_model);
                            if (comments_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin comment thất bại");
                            }

                            break;
                        }
                    case QueueType.INSERT_PRODUCT_RATING:
                        {
                            var model = JsonConvert.DeserializeObject<ProductRaitingPushQueueModel>(queue_info.data_push);
                            var id = await productRaitingService.InsertRaiting(model);
                            if (id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin raiting thất bại");
                            }
                            workQueueClient.SyncES(id, "SP_GetRating", "hulotoys_sp_getrating", Convert.ToInt16(ProjectType.HULOTOYS));
                            break;

                        }
                    case QueueType.UPDATE_ORDER:
                        {
                            var model = JsonConvert.DeserializeObject<OrderModel>(queue_info.data_push);
                            var id = await orderBusiness.UpdateOrder(model);
                            if (id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Cập nhật thông tin đơn hàng thất bại");
                            }
                            workQueueClient.SyncES(id, "SP_GetOrder", "hulotoys_sp_getorder", Convert.ToInt16(ProjectType.HULOTOYS));

                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "DoSomeRealWork = " + ex.ToString());
            }
        }
    }
}

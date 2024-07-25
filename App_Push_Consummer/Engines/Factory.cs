using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Engines.Address;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Queue;
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

        public Factory(IAddressBusiness _address_business, IAccountClientBusiness _accountclient_business)
        {
            address_business = _address_business;
            accountclient_business = _accountclient_business;
        }

        public async void DoSomeRealWork(string data_queue)
        {
            try
            {
                var queue_info = JsonConvert.DeserializeObject<QueueModel>(data_queue);
                switch (queue_info.queue_type)
                {
                    case QueueType.ADD_ADDRESS:
                        {
                            var address_model = JsonConvert.DeserializeObject<AddressModel>(queue_info.data_receiver);
                            var address_id = await address_business.saveAddressClient(address_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin địa chỉ thất bại");
                            }
                            break;
                        }
                    case QueueType.UPDATE_ADDRESS:
                        {
                            var address_model = JsonConvert.DeserializeObject<AddressModel>(queue_info.data_receiver);
                            var address_id = await address_business.updateAddressClient(address_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Cập nhật thông tin địa chỉ thất bại");
                            }
                            break;
                        }
                    case QueueType.ADD_USER:
                        {
                            var accountclient_model = JsonConvert.DeserializeObject<AccountClientModel>(queue_info.data_receiver);
                            var address_id = await accountclient_business.saveAccountClient(accountclient_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin đăng ký người dùng thất bại");
                            }
                            break;
                        }
                    case QueueType.UPDATE_USER:
                        {
                            var accountclient_model = JsonConvert.DeserializeObject<AccountClientModel>(queue_info.data_receiver);
                            var address_id = await accountclient_business.updateAccountClient(accountclient_model);
                            if (address_id < 0)
                            {
                                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin xác nhận quên mật khẩu thất bại");
                            }
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

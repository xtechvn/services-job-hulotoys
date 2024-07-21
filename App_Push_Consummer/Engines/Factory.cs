using App_Push_Consummer.Behaviors;
using App_Push_Consummer.Common;
using App_Push_Consummer.Engines.Address;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Queue;
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

        public Factory(IAddressBusiness _address_business)
        {
            address_business = _address_business;
        }

        public async void DoSomeRealWork(string data_queue)
        {
            try
            {
                var queue_info = JsonConvert.DeserializeObject<QueueModel>(data_queue);
                switch (queue_info.queue_type)
                {
                    case QueueType.ADDRESS_DETAIL:
                        var address_model = JsonConvert.DeserializeObject<AddressModel>(queue_info.data_receiver);
                        var address_id = await address_business.saveAddress(address_model);
                        if (address_id < 0)
                        {
                            ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Lưu thông tin địa chỉ thất bại");
                        }
                        break;

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

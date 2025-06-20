using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.DB_Core;
using App_Push_Consummer.RabitMQ;
using System.Configuration;
using Utilities.Contants;

namespace App_Push_Consummer.Engines.Address
{
    public class AddressBusiness: IAddressBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        private readonly WorkQueueClient workQueueClient;
        public AddressBusiness()
        {
            workQueueClient = new WorkQueueClient();
        }
        public async Task<Int32> saveAddressClient(AddressModel data)
        {
			try
			{
                int response = Repository.saveAddressClient(data);
                workQueueClient.SyncES(-1, "SP_GetAddressClient", "hulotoys_sp_getaddressclient", Convert.ToInt16(ProjectType.HULOTOYS));
                return response;

			}
			catch (Exception ex)
			{
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveAddress => error queue = " + ex.ToString());
                return -1;                
			}
        }
        public async Task<Int32> updateAddressClient(AddressModel data)
        {
            try
            {
                int response = Repository.updateAddressClient(data);
                workQueueClient.SyncES(-1, "SP_GetAddressClient", "hulotoys_sp_getaddressclient", Convert.ToInt16(ProjectType.HULOTOYS));
                return response;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveAddress => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

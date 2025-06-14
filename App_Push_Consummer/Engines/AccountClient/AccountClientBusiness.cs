using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.DB_Core;
using App_Push_Consummer.RabitMQ;
using HuloToys_Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Contants;

namespace App_Push_Consummer.Engines.AccountClient
{
    public class AccountClientBusiness: IAccountClientBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        private readonly WorkQueueClient workQueueClient;
        public AccountClientBusiness()
        {
            workQueueClient = new WorkQueueClient();
        }
        public async Task<Int32> saveAccountClient(AccountClientModel data)
        {
            try
            {
                int InsertClient = Repository.saveClient(data);
                workQueueClient.SyncES(-1, "SP_GetClient", "hulotoys_sp_getclient", Convert.ToInt16(ProjectType.HULOTOYS));

                data.ClientId = InsertClient;
                int response = Repository.saveAccountClient(data);
                workQueueClient.SyncES(response, "SP_GetAccountClient", "hulotoys_sp_getaccountclient", Convert.ToInt16(ProjectType.HULOTOYS));

                return response;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveAccountClient => error queue = " + ex.ToString());
                return -1;
            }
        }
        public async Task<Int32> updateAccountClient(AccountClientModel data)
        {
            try
            {
                int response = Repository.updateAccountClient(data);
                workQueueClient.SyncES(response, "SP_GetAccountClient", "hulotoys_sp_getaccountclient", Convert.ToInt16(ProjectType.HULOTOYS));
                return response;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "updateAccountClient => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

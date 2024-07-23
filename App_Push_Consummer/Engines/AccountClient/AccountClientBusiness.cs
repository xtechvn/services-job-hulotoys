using App_Push_Consummer.Common;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.DB_Core;
using HuloToys_Service.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Engines.AccountClient
{
    public class AccountClientBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];

        public async Task<Int32> saveAccountClient(AccountClientViewModel data)
        {
            try
            {
                int response = Repository.saveAccountClient(data);
                return response;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveAccountClient => error queue = " + ex.ToString());
                return -1;
            }
        }
        public async Task<Int32> updateAccountClient(AccountClientViewModel data)
        {
            try
            {
                int response = Repository.updateAccountClient(data);
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

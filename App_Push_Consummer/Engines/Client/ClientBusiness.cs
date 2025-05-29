using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Client;
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

namespace App_Push_Consummer.Engines.Client
{
    public class ClientBusiness : IClientBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        private readonly WorkQueueClient workQueueClient;
        public ClientBusiness()
        {
            workQueueClient = new WorkQueueClient();
        }
        public async Task<Int32> UpdateClient(ClientDetailESModel data)
        {
            try
            {
                int InsertClient = Repository.UpdateClient(data);

                return InsertClient;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveAccountClient => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

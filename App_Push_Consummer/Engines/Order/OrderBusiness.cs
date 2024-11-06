using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.DB_Core;
using App_Push_Consummer.Model.Order;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Engines.Order
{
    public class  OrderBusiness: IOrderBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public async Task<Int32> UpdateOrder(OrderModel data)
        {
            try
            {
                var UpdateOrder = Repository.UpdateOrder(data);
                if (UpdateOrder == null)
                {
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "không tìm thấy client OrderId=" + data.OrderId.ToString());
                    return -1;
                }
               
                return (int)data.OrderId;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveComments => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

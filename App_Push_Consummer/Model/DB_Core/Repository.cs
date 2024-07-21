using App_Push_Consummer.Common;
using App_Push_Consummer.Model.Address;
using System.Configuration;
using System.Data.SqlClient;

namespace App_Push_Consummer.Model.DB_Core
{
    public static class Repository
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static int saveAddress(AddressModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[12];
                objParam_order[0] = new SqlParameter("@id", model.id);               

                var id = DBWorker.ExecuteNonQuery("sp_saveAddress", objParam_order);                
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

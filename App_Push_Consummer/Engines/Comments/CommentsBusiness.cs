using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Comments;
using App_Push_Consummer.Model.DB_Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Engines.Comments
{
    public class  CommentsBusiness: ICommentsBusiness
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public async Task<Int32> saveComments(CommentsModel data)
        {
            try
            {
                var client = Repository.GetClientByAccountClientId(data.AccountClientId);
                if (client == null)
                {
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "không tìm thấy client AccountClientId=" + data.AccountClientId.ToString());
                    return -1;
                }
                int response = Repository.saveComments(client.ClientId, data.Content);
                return response;

            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "saveComments => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

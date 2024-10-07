using App_Push_Consummer.Common;
using App_Push_Consummer.Interfaces;
using App_Push_Consummer.Model.Comments;
using App_Push_Consummer.Model.DB_Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEB.CMS.Models.Product;

namespace App_Push_Consummer.Engines.ProductRaiting
{
    public class ProductRaitingService : IProductRaitingService
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        private static ProductDetailMongoAccess productDetailMongoAccess = new ProductDetailMongoAccess();
        public ProductRaitingService() { 
        
        
        }

        public async Task<int> InsertRaiting(ProductRaitingPushQueueModel model)
        {
            try
            {
                var id= Repository.InsertProductRaiting(model);
                var raitings = Repository.GetRaitingByProductID(model.ProductId);
                if (raitings != null && raitings.Count > 0) {
                    float avarage = raitings.Sum(x => (float)x.Star) / raitings.Count;
                    await productDetailMongoAccess.UpdateStar(model.ProductId, avarage);

                }

                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "InsertRaiting => error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}

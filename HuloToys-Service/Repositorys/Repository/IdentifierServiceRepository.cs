using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using REPOSITORIES.IRepositories;

namespace REPOSITORIES.Repositories
{
    public class IdentifierServiceRepository : IIdentifierServiceRepository
    {
        

        public IdentifierServiceRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
         
        }

        /// <summary>
        ///  Cấu trúc mã đơn hàng: năm + tháng + ngày + 5 số tăng dần (24082300001). Cứ hết năm sẽ reset 5 số tăng dần.
        /// </summary>
        /// <returns></returns>
        public async Task<string> buildOrderNo(long count_exists_order =0)
        {
            try
            {
                if(count_exists_order > -1)
                {
                    count_exists_order++;
                    string order_no = string.Empty;
                    order_no += DateTime.Now.ToString("yyMMdd");
                    order_no += count_exists_order.ToString("D5");
                    return order_no;
                }
                
            }
            catch (Exception ex)
            {
               
            }
            string date = DateTime.Now.ToString("yyMMdd");
            var order_default = new Random().Next(1, 99999);
            return "HO" + date + order_default.ToString("D5");
        }


        

    }
}

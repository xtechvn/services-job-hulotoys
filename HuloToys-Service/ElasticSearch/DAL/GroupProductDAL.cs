using HuloToys_Service.ElasticSearch.NewEs;
using HuloToys_Service.Models.Products;
using HuloToys_Service.Utilities.Lib;

namespace HuloToys_Service.ElasticSearch.DAL
{
    public class GroupProductDAL 
    {
        public IConfiguration configuration;
        private readonly GroupProductESService groupProductESService;

        public GroupProductDAL(string connection, IConfiguration _configuration) 
        {
            configuration = _configuration;
            groupProductESService = new GroupProductESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }

        public List<GroupProduct> GetByParentId(long parent_id)
        {
            try
            {
                var data = groupProductESService.GetListGroupProductByParentId(parent_id);
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByParentId-GroupProductDAL" + ex);
            }
            return null;
        }
        public GroupProduct GetById(long id)
        {
            try
            {
                var data = groupProductESService.GetDetailGroupProductById(id);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetById-GroupProductDAL" + ex);

            }
            return null;
        }
    }
}

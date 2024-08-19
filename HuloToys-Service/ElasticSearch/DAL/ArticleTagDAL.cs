using HuloToys_Service.ElasticSearch.NewEs;
using HuloToys_Service.Models.Entities;
using HuloToys_Service.Utilities.Lib;
using Newtonsoft.Json;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch.DAL
{
    public class ArticleTagDAL
    {

        public IConfiguration configuration;
        public ArticleTagESService articleTagESService;
        public ArticleTagDAL(string connection, IConfiguration _configuration)
        {
            configuration = _configuration;
            articleTagESService = new ArticleTagESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }
        public List<long> GetTagIDByArticleID(long articleID)
        {
            try
            {

                var data = articleTagESService.GetListArticleTagByArticleId(articleID);
                var List_TagId = data.Select(s => s.TagId);
                if (List_TagId != null && List_TagId.Count() > 0)
                {
                    var json = JsonConvert.SerializeObject(List_TagId.Distinct().ToList());
                    return JsonConvert.DeserializeObject<List<long>>(json);
                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
    }
}

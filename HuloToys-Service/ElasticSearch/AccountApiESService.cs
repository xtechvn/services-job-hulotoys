using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Account;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;
using Telegram.Bot.Types;

namespace HuloToys_Service.ElasticSearch
{
    public class AccountApiESService : ESRepository<AccountApiESModel>
    {
        public string index = "hulotoys_sp_get_accountaccessapi";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public AccountApiESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;

        }
        public AccountApiESModel GetByUsername(string user_name)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                LogHelper.InsertLogTelegram(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByUsername - AccountApiESService:" + user_name);

                var query = elasticClient.Search<AccountApiESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("username").Query(user_name)
                               )));

                if (query.IsValid)
                {
                    var result = query.Documents as List<AccountApiESModel>;
                    LogHelper.InsertLogTelegram(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByUsername - AccountApiESService Count:" +result.Count);

                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByUsername - AccountApiESService Error" + ex.ToString());
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegram(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
    }
}

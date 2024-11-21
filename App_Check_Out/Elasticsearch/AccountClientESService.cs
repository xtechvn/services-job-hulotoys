using Elasticsearch.Net;
using Nest;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using APP_CHECKOUT.Models.Account;
using APP_CHECKOUT.Utilities.Lib;

namespace APP_CHECKOUT.Elasticsearch
{
    public class AccountClientESService : ESRepository<AccountESModel>
    {
        public string index = "account_client_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public AccountClientESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["Elastic:Index:AccountClient"];

        }
        public AccountESModel GetByUsername(string user_name)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<AccountESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("UserName").Query(user_name)
                               )));

                if (query.IsValid)
                {
                    var result = query.Documents as List<AccountESModel>;
                    //var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
      
        public AccountESModel GetById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<object>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("Id").Query(id.ToString())
                               )));

                if (query.IsValid)
                {
                    var result = query.Documents as List<object>;
                    var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public AccountESModel GetByUsernameAndPassword(string user_name, string password)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<object>(sd => sd
                               .Index(index)
                             .Query(q =>
                               q.Bool(
                                   qb => qb.Must(
                                      qb => qb.Term("UserName", user_name),
                                       qb => qb.Term("Password", password)

                                    )
                               )
                            ));


                if (query.IsValid)
                {
                    var result = query.Documents as List<object>;
                    var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));

                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public AccountESModel GetByUsernameAndGoogleToken(string user_name, string token)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<object>(sd => sd
                               .Index(index)
                             .Query(q =>
                               q.Bool(
                                   qb => qb.Must(
                                      qb => qb.Term("UserName", user_name)

                                    )
                               )
                            ));


                if (query.IsValid)
                {
                    var result = query.Documents as List<object>;
                    var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public AccountESModel GetByClientIdAndPassword(long client_id, string password)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<object>(sd => sd
                               .Index(index)
                             .Query(q =>
                               q.Bool(
                                   qb => qb.Must(
                                      qb => qb.Term("ClientId", client_id),
                                       qb => qb.Term("Password", password)

                                    )
                               )
                            ));


                if (query.IsValid)
                {
                    var result = query.Documents as List<object>;
                    var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public AccountESModel GetByClientID(long client_id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);


                var query = elasticClient.Search<object>(sd => sd
                             .Index(index)
                           .Query(q =>
                             q.Bool(
                                 qb => qb.Must(
                                    qb => qb.Term("ClientId", client_id)


                                  )
                             )
                          ));

                if (query.IsValid)
                {
                    var result = query.Documents as List<object>;
                    var data = JsonConvert.DeserializeObject<List<AccountESModel>>(JsonConvert.SerializeObject(result));
                    return data.FirstOrDefault();
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

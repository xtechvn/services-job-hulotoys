using Elasticsearch.Net;
using Nest;
using System.Reflection;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using APP_CHECKOUT.Utilities.Lib;

namespace APP_CHECKOUT.Elasticsearch
{
    public class AddressClientESService : ESRepository<AddressClientESModel>
    {
        public string index = "address_client_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public AddressClientESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["Elastic:Index:AddressClient"];


        }
        public List<AddressClientESModel> GetByClientID(long client_id)
        {
            List<AddressClientESModel> result = new List<AddressClientESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<object>(sd => sd
                            .Index(index)
                            .Query(q => q
                                .Match(m => m.Field("ClientId").Query(client_id.ToString())
                                ))
                            .Size(100)

                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    var data = query.Documents as List<AddressClientESModel>;
                    return data;
                }
            }
            catch (Exception ex)
            {
                string error_msg = "AddressClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public AddressClientESModel GetById(long id,long client_id)
        {
            AddressClientESModel result = new AddressClientESModel();
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
                                ) 
                                && 
                                q.Match(m => m.Field("ClientId").Query(client_id.ToString()))
                                )
                            .Size(100)

                            );

                if (!query.IsValid)
                {
                    return null;
                }
                else
                {

                    var data = query.Documents as List<AddressClientESModel>;
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = "AddressClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
    }
}

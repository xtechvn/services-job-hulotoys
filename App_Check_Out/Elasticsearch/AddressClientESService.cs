using Elasticsearch.Net;
using Nest;
using System.Reflection;
using Entities.Models;
using APP_CHECKOUT.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace Caching.Elasticsearch
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
                var query = elasticClient.Search<AddressClientESModel>(sd => sd
                            .Index(index)
                            .Query(q => q
                                .Match(m => m.Field(x=>x.clientid).Query(client_id.ToString())
                                ))
                            .Size(100)

                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<AddressClientESModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public AddressClientESModel GetById(long id)
        {
            AddressClientESModel result = new AddressClientESModel();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<AddressClientESModel>(sd => sd
                            .Index(index)
                            .Query(q => q
                                .Match(m => m.Field(x => x.id).Query(id.ToString())
                                ) 
                                )
                            .Size(100)

                            );

                if (!query.IsValid)
                {
                    return null;
                }
                else
                {
                    var list = query.Documents as List<AddressClientESModel>;
                    return list.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}

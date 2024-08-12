using HuloToys_Service.Elasticsearch;
using Entities.Models;
using Elasticsearch.Net;
using Nest;
using System.Reflection;
using HuloToys_Service.Utilities.Lib;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Models.Entities;

namespace HuloToys_Service.ElasticSearch.LocationProduct
{
    public class LocationProductESService : ESRepository<Entities.Models.LocationProduct>
    {
        public string index = "location_product_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public LocationProductESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:LocationProduct"];
        }

        public List<Entities.Models.LocationProduct> GetListByGroupId(long groupproductid)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<LocationProductESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("groupproductid").Query(groupproductid.ToString())
                               )));

                if (query.IsValid)
                {
                    var data = query.Documents as List<LocationProductESModel>;

                    var result = data.Select(a => new Entities.Models.LocationProduct
                    {

                        LocationProductId = a.locationproductid,
                        ProductCode = a.productcode,
                        GroupProductId = a.groupproductid,
                        OrderNo = a.orderno,
                        CreateOn = a.createon,
                        UpdateLast = a.updatelast,
                        UserId = a.userid,
           

                    }).ToList();
                    return result;
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

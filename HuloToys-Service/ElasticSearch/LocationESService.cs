using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;
using HuloToys_Service.Models.Orders;
using HuloToys_Service.Models.Location;
using Utilities.Contants;

namespace Caching.Elasticsearch
{
    public class LocationESService : ESRepository<ProvinceESModel>
    {
        public string index_province = "provinces_store";
        public string index_district = "districts_store";
        public string index_wards = "wards_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public LocationESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;

        }
        public List<ProvinceESModel> GetAllProvinces()
        {
            List<ProvinceESModel> result = new List<ProvinceESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<ProvinceESModel>(sd => sd
                            .Index(index_province)
                            .Query(q => q
                                .MatchAll()
                                )
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<ProvinceESModel>;
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
        public List<DistrictESModel> GetAllDistrict()
        {
            List<DistrictESModel> result = new List<DistrictESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<DistrictESModel>(sd => sd
                            .Index(index_district)
                            .Query(q => q
                                .MatchAll()
                                )
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<DistrictESModel>;
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
        public List<DistrictESModel> GetAllDistrictByProvinces(string provinces_id)
        {
            List<DistrictESModel> result = new List<DistrictESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<DistrictESModel>(sd => sd
                            .Index(index_district)
                            .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field(x => x.provinceid).Query(provinces_id)
                                   )
                                   
                                )))
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<DistrictESModel>;
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
        public List<WardESModel> GetAllWards()
        {
            List<WardESModel> result = new List<WardESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<WardESModel>(sd => sd
                            .Index(index_wards)
                            .Query(q => q
                                .MatchAll()
                                )
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<WardESModel>;
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
        public List<WardESModel> GetAllWardsByDistrictId(string district_id)
        {
            List<WardESModel> result = new List<WardESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<WardESModel>(sd => sd
                            .Index(index_wards)
                            .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field(x => x.districtid).Query(district_id)
                                   )

                                ))
                                )
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<WardESModel>;
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

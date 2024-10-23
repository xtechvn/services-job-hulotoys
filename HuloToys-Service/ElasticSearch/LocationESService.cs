using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;
using HuloToys_Service.Models.Orders;
using HuloToys_Service.Models.Location;
using Utilities.Contants;
using Newtonsoft.Json;

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
        public List<Province> GetAllProvinces()
        {
            List<Province> result = new List<Province>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_province)
                            .Size(4000)
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
                    //result = query.Documents as List<Province>;
                    result = JsonConvert.DeserializeObject<List<Province>>(JsonConvert.SerializeObject(query.Documents));
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
        public Province GetProvincesByProvinceId(string provinces_id)
        {
            List<Province> result = new List<Province>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_province)
                            .Size(4000)
                           .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("ProvinceId").Query(provinces_id)
                                   )

                                )))
                            );

                if (!query.IsValid)
                {
                    return null;
                }
                else
                {
                    //result = query.Documents as List<Province>;
                    result = JsonConvert.DeserializeObject<List<Province>>(JsonConvert.SerializeObject(query.Documents));
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
        public List<District> GetAllDistrict()
        {
            List<District> result = new List<District>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_district)
                             .Size(4000)
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
                    //result = query.Documents as List<District>;
                    result = JsonConvert.DeserializeObject<List<District>>(JsonConvert.SerializeObject(query.Documents));

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
        public List<District> GetAllDistrictByProvinces(string provinces_id)
        {
            List<District> result = new List<District>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_district)
                            .Size(4000)
                            .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("ProvinceId").Query(provinces_id)
                                   )

                                )))
                            );

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    //result = query.Documents as List<District>;
                    result = JsonConvert.DeserializeObject<List<District>>(JsonConvert.SerializeObject(query.Documents));

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
        public District GetDistrictByDistrictId(string district_id)
        {
            List<District> result = new List<District>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_district)
                            .Size(4000)
                           .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("DistrictId").Query(district_id)
                                   )

                                ))
                                )
                            );

                if (!query.IsValid)
                {
                    return null;
                }
                else
                {
                    //result = query.Documents as List<District>;
                    result = JsonConvert.DeserializeObject<List<District>>(JsonConvert.SerializeObject(query.Documents));

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
        public List<Ward> GetAllWards()
        {
            List<Ward> result = new List<Ward>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_wards)
                            .Size(4000)
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
                    //result = query.Documents as List<Ward>;
                    result = JsonConvert.DeserializeObject<List<Ward>>(JsonConvert.SerializeObject(query.Documents));

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
        public List<Ward> GetAllWardsByDistrictId(string district_id)
        {
            List<Ward> result = new List<Ward>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_wards)
                            .Size(4000)
                            .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("DistrictId").Query(district_id)
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
                    //result = query.Documents as List<Ward>;
                    result = JsonConvert.DeserializeObject<List<Ward>>(JsonConvert.SerializeObject(query.Documents));

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
        public Ward GetWardsByWardId(string ward_id)
        {
            List<Ward> result = new List<Ward>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var query = elasticClient.Search<dynamic>(sd => sd
                            .Index(index_wards)
                            .Size(4000)
                            .Query(q => q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("WardId").Query(ward_id)
                                   )

                                ))
                                )
                            );

                if (!query.IsValid)
                {
                    return null;
                }
                else
                {
                    //result = query.Documents as List<Ward>;
                    result = JsonConvert.DeserializeObject<List<Ward>>(JsonConvert.SerializeObject(query.Documents));

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
    }
}

﻿using Elasticsearch.Net;
using Models.ElasticSearch;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Utilities.Lib;
using Nest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using Utilities;

namespace Caching.Elasticsearch
{
    public class OrderESService : ESRepository<OrderESModel>
    {
        public string index = "order_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public OrderESService(string Host,IConfiguration _configuration) : base(Host, _configuration) {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Order"];


        }
        public List<OrderESModel> GetByClientID( long client_id)
        {
            List<OrderESModel> result = new List<OrderESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<OrderESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("clientid").Query(client_id.ToString())
                                   )));

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    result = query.Documents as List<OrderESModel>;
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
        public OrderESModel GetLastestClientID(long client_id)
        {
            OrderESModel result = new OrderESModel();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<OrderESModel>(sd => sd
                               .Index(index)
                               
                               .Query(q => q
                                   .Match(m => m.Field("clientid").Query(client_id.ToString())
                                   ))
                                .Sort(q => q.Descending(u => u.createtime))); ;

                if (!query.IsValid)
                {
                    return result;
                }
                else
                {
                    var rs=  query.Documents as List<OrderESModel>;
                    return rs.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public List<OrderESModel> GetByOrderNo(string text,long client_id)
        {
            List<OrderESModel> result = new List<OrderESModel>();
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderESModel>(s => s
                        .Index(index)
                        .Size(4000)
                        .Query(q =>
                         q.Bool(
                             qb => qb.Must(
                                 q => q.Term("clientid", client_id.ToString()),
                                 q => q.QueryString(qs => qs
                                 .Fields(new[] { "orderno" })
                                 .Query("*" + text.ToUpper() + "*")
                                 .Analyzer("standard")

                          )
                         )
                        )));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<OrderESModel>;
                    return result ?? new List<OrderESModel>();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return new List<OrderESModel>();
        }
    }
}
﻿using Elasticsearch.Net;
using Nest;
using System.Reflection;
using Newtonsoft.Json;
using APP_CHECKOUT.Utilities.Lib;
using System.Configuration;
using APP_CHECKOUT.Models.Client;

namespace APP_CHECKOUT.Elasticsearch
{
    public class ClientESService : ESRepository<ClientESModel>
    {
        public string index = "client_hulotoys_store";
        private static string _ElasticHost;

        public ClientESService(string Host) : base(Host) {
            _ElasticHost = Host;
            index = ConfigurationManager.AppSettings["Elastic_Index_Client"];

        }
        public ClientESModel GetById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ClientESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("Id").Query(id.ToString())
                               )));

                if (query.IsValid)
                {
                    var result = query.Documents as List<ClientESModel>;
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg =  "ClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(error_msg);
            }
            return null;
        }
        public List<ClientESModel> GetByEmail(string email)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ClientESModel>(sd => sd
                               .Index(index)
                                .Query(q => q
                                .MatchPhrase(m => m
                                .Field(f => f.Email)
                                .Query(email))));
               
                if (query.IsValid)
                {
                    var result = query.Documents as List<ClientESModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                string error_msg = "ClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(error_msg);
            }
            return null;
        }
        public List<ClientESModel> GetByPhone(string phone)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<dynamic>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("Phone").Query(phone)
                               )));

                if (query.IsValid)
                {
                    var result = query.Documents as List<dynamic>;
                    var data = JsonConvert.DeserializeObject<List<ClientESModel>>(JsonConvert.SerializeObject(result));
                    return data;
                }
            }
            catch (Exception ex)
            {
                string error_msg = "ClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(error_msg);
            }
            return null;
        }
        public long GetCountClientTypeUse(int client_type)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<dynamic>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("ClientType").Query(client_type.ToString())
                               )));

                if (query.IsValid)
                {
                    var data = query.Documents as List<dynamic>;
                    var result = JsonConvert.DeserializeObject<List<ClientESModel>>(JsonConvert.SerializeObject(data));
                    return result.Count;
                }
            }
            catch (Exception ex)
            {
                string error_msg = "ClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(error_msg);
            }
            return 0;
        }
        public ClientESModel GetExactByEmail(string email)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<dynamic>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("Email").Query(email)
                               )));

                if (query.IsValid)
                {
                    var data = query.Documents as List<dynamic>;
                    var result = JsonConvert.DeserializeObject<List<ClientESModel>>(JsonConvert.SerializeObject(data));
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = "ClientESService ->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(error_msg);
            }
            return null;
        }
    }
}

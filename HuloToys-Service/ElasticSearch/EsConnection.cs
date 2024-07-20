﻿using Elasticsearch.Net;
using Nest;

namespace HuloToys_Service.Elasticsearch
{
    public class EsConnection
    {
        private readonly string _ElasticHost;

        public EsConnection(string _EsHost)
        {
            _ElasticHost = _EsHost;
        }

        /// <summary>
        /// Connect toi ES
        /// </summary>
        /// <param name="_ElasticHost"></param>
        /// <param name="index_name"></param>
        /// <param name="msg_connection"></param>
        /// <returns></returns>
        public ElasticClient connectES(string index_name, out string msg_connection)
        {
            msg_connection = string.Empty;
            try
            {
                var elasticClient = new ElasticClient();
                // Connection URL's ES
                var listOfUrl = new Uri[] {
                    new Uri(_ElasticHost)
                };

                var connPool = new StaticConnectionPool(listOfUrl);
                var connSett = new ConnectionSettings(connPool);
                var eClient = new ElasticClient(connSett);

                // Check Connection Health
                var checkClusterHealth = eClient.Cluster.Health();
                if (checkClusterHealth.ApiCall.Success && checkClusterHealth.IsValid)
                {
                    //2, Check the index Exists or not
                    var checkResult = eClient.Indices.Exists(index_name);
                    if (!checkResult.Exists)
                    {                        
                        msg_connection = "_ElasticHost = " + _ElasticHost + "--> Index " + index_name + "in ES Not Avaliable !!!";
                        return null;
                    }
                    else
                    {
                        return eClient;
                    }
                }
                else
                {
                    msg_connection = "checkClusterHealth not is vaild";
                    return null;
                }
            }
            catch (Exception ex)
            {
                msg_connection = ex.ToString();
                return null;
            }
        }

        //public ElasticClient EsClient()
        //{
        //    var elasticClient = new ElasticClient();
        //    try
        //    {
        //        var nodes = new Uri[] { new Uri(_ElasticHost) };
        //        var connectionPool = new StaticConnectionPool(nodes);
        //        var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
        //        elasticClient = new ElasticClient(connectionSettings);
        //    }
        //    catch
        //    {

        //    }
        //    return elasticClient;
        //}

        

    }
}

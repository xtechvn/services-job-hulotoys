using Elasticsearch.Net;
using Entities.Models;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Models.Entities;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch.NewEs
{
    public class ArticleESService : ESRepository<Article>
    {
        public string index = "article_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public ArticleESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Article"];
        }
        public Article GetDetailById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleESModel>(sd => sd
                               .Index(index)
                               .Query(q => q
                                   .Match(m => m.Field("id").Query(id.ToString())
                               )));

                if (query.IsValid)
                {

                    var data = query.Documents as List<ArticleESModel>;
                    var result = data.Select(a => new Article
                    {

                        Id = a.id,
                        Title = a.title,
                        Lead = a.lead,
                        Body = a.body,
                        Status = a.status,
                        ArticleType = a.articletype,
                        PageView = a.pageview,
                        PublishDate = a.publishdate,
                        AuthorId = a.authorid,
                        Image169 = a.image169,
                        Image43 = a.image43,
                        Image11 = a.image11,
                        CreatedOn = a.createdon,
                        ModifiedOn = a.modifiedon,
                        DownTime = a.downtime,
                        UpTime = a.uptime,
                        Position = a.position,

                    }).ToList();
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
        public List<Article> GetListArticle()
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q.MatchAll()
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleESModel>;
                    var result = data.Select(a => new Article
                    {

                        Id = a.id,
                        Title = a.title,
                        Lead = a.lead,
                        Body = a.body,
                        Status = a.status,
                        ArticleType = a.articletype,
                        PageView = a.pageview,
                        PublishDate = a.publishdate,
                        AuthorId = a.authorid,
                        Image169 = a.image169,
                        Image43 = a.image43,
                        Image11 = a.image11,
                        CreatedOn = a.createdon,
                        ModifiedOn = a.modifiedon,
                        DownTime = a.downtime,
                        UpTime = a.uptime,
                        Position = a.position,

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
        public List<Article> GetListArticlePosition()
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q
                                   .Range(m => m.Field("position").GreaterThanOrEquals(1).LessThanOrEquals(7)
                               )));
                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleESModel>;
                    var result = data.Select(a => new Article
                    {

                        Id = a.id,
                        Title = a.title,
                        Lead = a.lead,
                        Body = a.body,
                        Status = a.status,
                        ArticleType = a.articletype,
                        PageView = a.pageview,
                        PublishDate = a.publishdate,
                        AuthorId = a.authorid,
                        Image169 = a.image169,
                        Image43 = a.image43,
                        Image11 = a.image11,
                        CreatedOn = a.createdon,
                        ModifiedOn = a.modifiedon,
                        DownTime = a.downtime,
                        UpTime = a.uptime,
                        Position = a.position,

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

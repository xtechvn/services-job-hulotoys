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
    public class ArticleCategoryESService : ESRepository<ArticleCategory>
    {
        public string index = "article_category_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public ArticleCategoryESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:ArticleCategory"];

        }
        public List<ArticleCategory> GetByArticleId(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleCategoryESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q
                                   .Match(m => m.Field("articleid").Query(id.ToString())
                               )));

                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleCategoryESModel>;
                    var result = data.Select(a => new ArticleCategory
                    {

                        Id = a.id,
                        ArticleId = a.articleid,
                        CategoryId = a.categoryid,


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
        public List<ArticleCategory> GetListArticleCategory()
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleCategoryESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q.MatchAll()
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleCategoryESModel>;
                    var result = data.Select(a => new ArticleCategory
                    {

                        Id = a.id,
                        ArticleId = a.articleid,
                        CategoryId = a.categoryid,


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
        public List<ArticleCategory> GetByCategoryId(long CategoryId)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleCategoryESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q
                                   .Match(m => m.Field("categoryid").Query(CategoryId.ToString())
                               )));

                if (query.IsValid)
                {

                    var data = query.Documents as List<ArticleCategoryESModel>;
                    var result = data.Select(a => new ArticleCategory
                    {

                        Id = a.id,
                        ArticleId = a.articleid,
                        CategoryId= a.categoryid,


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

using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class ArticleESService : ESRepository<ArticleViewModel>
    {
        public string index = "hulotoys_sp_getarticle";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public ArticleESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Article"];
        }
        public ArticleESModel GetDetailById(long id)
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
                                   .Term(m => m.Field("Id").Value(id)
                               )));

                if (query.IsValid)
                {

                    var data = query.Documents as List<ArticleESModel>;

                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public List<ArticleESModel> GetListArticle()
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleESModel>(sd => sd
                               .Index(index)
                               .Size(100)
                               .Query(q => q.MatchAll()
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleESModel>;
                    //var result = data.Select(a => new ArticleViewModel
                    //{

                    //    Id = a.id,
                    //    Title = a.title,
                    //    Lead = a.lead,
                    //    Body = a.body,
                    //    Status = a.status,
                    //    ArticleType = a.articletype,
                    //    PageView = a.pageview,
                    //    PublishDate = a.publishdate,
                    //    AuthorId = a.authorid,
                    //    Image169 = a.image169,
                    //    Image43 = a.image43,
                    //    Image11 = a.image11,
                    //    CreatedOn = a.createdon,
                    //    ModifiedOn = a.modifiedon,
                    //    DownTime = a.downtime,
                    //    UpTime = a.uptime,
                    //    Position = a.position,

                    //}).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public List<ArticleESModel> GetListArticlePosition()
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
                                   .Range(m => m.Field("Position").GreaterThanOrEquals(1).LessThanOrEquals(7)
                               )));
                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleESModel>;
                    //var result = data.Select(a => new ArticleViewModel
                    //{

                    //    Id = a.id,
                    //    Title = a.title,
                    //    Lead = a.lead,
                    //    Body = a.body,
                    //    Status = a.status,
                    //    ArticleType = a.articletype,
                    //    PageView = a.pageview,
                    //    PublishDate = a.publishdate,
                    //    AuthorId = a.authorid,
                    //    Image169 = a.image169,
                    //    Image43 = a.image43,
                    //    Image11 = a.image11,
                    //    CreatedOn = a.createdon,
                    //    ModifiedOn = a.modifiedon,
                    //    DownTime = a.downtime,
                    //    UpTime = a.uptime,
                    //    Position = a.position,

                    //}).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public List<ArticleRelationModel> GetListArticleByBody(string txt_search)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<ArticleESModel>(sd => sd
                               .Index(index)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  //q => q.Term("id", id),
                                   sh => sh.QueryString(qs => qs
                                   .Fields(new[] { "Title", "Lead", "Body" })
                                   .Query("*" + txt_search + "*")
                                   .Analyzer("standard")

                            )
                           )
                               )));

                if (query.IsValid)
                {

                    var data = query.Documents as List<ArticleESModel>;
                    var result = data.Select(a => new ArticleRelationModel
                    {
                        Id = a.Id,
                        Lead = a.Lead,
                        Image = a.Image169 ?? a.Image43 ?? a.Image11,
                        Title = a.Title,
                        publish_date = a.PublishDate ?? DateTime.Now,
                    }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
    }
}

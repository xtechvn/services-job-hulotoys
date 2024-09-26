using Azure.Core;
using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Models.Raiting;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Drawing;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class RaitingESService : ESRepository<RatingESModel>
    {
        public string index = "raiting_hulotoys_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;
        private static ElasticClient elasticClient;

        public RaitingESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Raiting"];
            var nodes = new Uri[] { new Uri(_ElasticHost) };
            var connectionPool = new StaticConnectionPool(nodes);
            var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index);
            elasticClient = new ElasticClient(connectionSettings);
        }
        public List<RatingESModel> GetListByFilter(ProductRaitingRequestModel request)
        {
            try
            {
               
                var searchRequest = new SearchRequest<RatingESModel>
                {
                    From = (request.page_index - 1) <0?0: (request.page_index - 1)*request.page_size, // Set the starting point of the result set
                    Size = request.page_size, // Set the number of documents to return
                    Query = new BoolQuery
                    {
                        Must = new List<QueryContainer>(),
                        Should=new List<QueryContainer>()
                    }
                };
                // Add filters conditionally
                var mustQueries = new List<QueryContainer>();
                var shouldQueries = new List<QueryContainer>();

                mustQueries.Add(new MatchQuery
                {
                    Field = Infer.Field<RatingESModel>(x => x.productid),
                    Query=request.id
                });

                if (request.stars>0)
                {
                    mustQueries.Add(new TermQuery
                    {
                        Field = Infer.Field<RatingESModel>(x => x.star),
                        Value = request.stars
                    });
                }
                if (request.has_comment ==true)
                {
                    mustQueries.Add(new ExistsQuery
                    {
                        Field = Infer.Field<RatingESModel>(x => x.comment)
                    });
                }
                if (request.has_media == true)
                {
                    var existsQuery = new BoolQuery
                    {
                        Should = new List<QueryContainer>
                        {
                            new ExistsQuery { Field = Infer.Field<RatingESModel>(x => x.imglink) },  // Check if 'age' exists
                            new ExistsQuery { Field = Infer.Field<RatingESModel>(x => x.videolink) }  // Check if 'name' exists
                        },
                        MinimumShouldMatch = 1 // At least one field ('age' or 'name') must exist
                    };
                    mustQueries.Add(existsQuery);
                }

                // Combine the conditions in the 'must' clause
                searchRequest.Query = new BoolQuery
                {
                    Must = mustQueries,
                    Should=shouldQueries
                };

               // Execute search
                var response = elasticClient.Search<RatingESModel>(searchRequest);
                var resultList = new List<RatingESModel>();

                foreach (var hit in response.Hits)
                {
                    resultList.Add((RatingESModel)hit.Source);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public ProductRaitingResponseModel CountCommentByProductId(string product_id)
        {
            ProductRaitingResponseModel result = new ProductRaitingResponseModel();
            try
            {
                // Build the search request
                var searchRequest = new SearchRequest<RatingESModel>
                {
                    Query = new TermQuery
                    {
                        Field = Infer.Field<RatingESModel>(p => p.productid), // Filter by product ID
                        Value = product_id
                    },
                    Size = 0, // No hits needed, just aggregations
                    Aggregations = new AggregationDictionary
                    {
                        {
                            "stars_count", new TermsAggregation("stars_count")
                            {
                                Field = Infer.Field<RatingESModel>(x => x.star), // Use 'Stars' field for aggregation
                                Size = 5, // Expecting 6 possible buckets (1 to 5 stars)
                                Order = new List<TermsOrder> // Ensure results are ordered by star value
                                {
                                    TermsOrder.KeyAscending
                                }
                            }
                        },
                        // Filters aggregation to count documents that have data in 'description' and 'information' fields
                        {
                            "media_count", new FiltersAggregation("media_count")
                            {
                                Filters = new List<QueryContainer>
                                {
                                    // BoolQuery to check if either 'description' or 'information' exists
                                    new BoolQuery
                                    {
                                        Should = new List<QueryContainer>
                                        {
                                            new ExistsQuery { Field = Infer.Field<RatingESModel>(x => x.imglink) }, // Check for 'description'
                                            new ExistsQuery { Field = Infer.Field<RatingESModel>(x => x.videolink) }  // Check for 'information'
                                        },
                                        MinimumShouldMatch = 1 // At least one of 'description' or 'information' must exist
                                    }
                                }
                            }
                        },
                        {
                            "comment_count", new FiltersAggregation("comment_count")
                            {
                                Filters = new List<QueryContainer>
                                {
                                    new ExistsQuery { Field = Infer.Field<RatingESModel>(x => x.comment) }, // Count products with 'description'
                                }
                            }
                        },
                        {
                            "total_product_count", new ValueCountAggregation("total_product_count", Infer.Field<RatingESModel>(x => x.id))
                        }
                    }
                };
                var response = elasticClient.Search<RatingESModel>(searchRequest);

                // Process the aggregation results
                var starCounts = new Dictionary<int, long>();
                var starsAgg = response.Aggregations.Terms("stars_count");

                foreach (var bucket in starsAgg.Buckets)
                {
                    // Convert the bucket key to an integer and store the document count
                    starCounts[int.Parse(bucket.Key)] = bucket.DocCount ?? 0;
                }

                result.comment_count_by_star = starCounts;

                // Process the field data counts (description and information)
                var fieldDataAgg = response.Aggregations.Filters("media_count");
                result.has_media_count = fieldDataAgg.Buckets.First().DocCount ; // Products with 'description' field

                // Process the field data counts (description and information)
                var comment_agg = response.Aggregations.Filters("comment_count");
                result.has_comment_count = comment_agg.Buckets.First().DocCount; // Products with 'description' field
                                                                                 // Process the field data counts (description and information)
                var total_product_count = response.Aggregations.Filters("total_product_count");
                result.total_count = comment_agg.Buckets.First().DocCount; // Products with 'description' field

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return result;
        }

    }

}

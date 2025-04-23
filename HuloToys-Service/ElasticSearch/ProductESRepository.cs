using HuloToys_Service.Elasticsearch;
using Nest;
using Utilities;

namespace Caching.Elasticsearch
{
    //https://www.steps2code.com/post/how-to-use-elasticsearch-in-csharp
    public class ProductESRepository : ESRepository<ProductESModel>
    {
        public string index = "hulotoys_mongodb_product";
        private static string _ElasticHost;
        private static IConfiguration configuration;
        private readonly ElasticClient _client;

        public ProductESRepository(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Product"];

            var settings = new ConnectionSettings(new Uri(_ElasticHost))
                .DefaultIndex(index);
            _client = new ElasticClient(settings);
        }

        // 1. Function tìm kiếm data theo product_id
        public async Task<ProductESModel> GetByProductIdAsync(string productId)
        {
            var response = await _client.SearchAsync<ProductESModel>(s => s
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.product_id)
                        .Value(productId)
                    )
                )
            );

            return response.Documents.FirstOrDefault();
        }

        // 2. Function xóa theo product_id
        public async Task<bool> DeleteByProductIdAsync(string productId)
        {
            var response = await _client.DeleteByQueryAsync<ProductESModel>(q => q
                .Query(rq => rq
                    .Term(t => t
                        .Field(f => f.product_id)
                        .Value(productId)
                    )
                )
            );

            return response.IsValid && response.Deleted > 0;
        }

        // 3. Function insert vào index
        public async Task<bool> InsertAsync(ProductESModel product)
        {
            var response = await _client.IndexDocumentAsync(product);
            return response.IsValid;
        }
        // Tìm kiếm theo keyword trên trường name và product_code
        public async Task<List<ProductESModel>> SearchByKeywordAsync(string keyword, string keywordNoSpace)
        {
            //var response = await _client.SearchAsync<ProductESModel>(s => s
            //    .Query(q => q
            //        .MultiMatch(m => m
            //            .Fields(f => f
            //                .Field(p => p.name)
            //                .Field(p => p.product_code)
            //            )
            //            .Query(keyword)
            //            .Type(TextQueryType.BestFields)
            //            .Analyzer("standard") // Sử dụng analyzer chuẩn hỗ trợ Unicode
            //            .Fuzziness(Fuzziness.Auto) // Cho phép tìm gần đúng
            //        )
            //    )
            //);
            var response = await _client.SearchAsync<ProductESModel>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            // Tìm kiếm chính xác với boost cao
                            sh => sh.MatchPhrase(m => m
                                .Field(p => p.name)
                                .Query(keyword)
                                .Boost(10)
                            ),
                            // Tìm kiếm từng từ
                            sh => sh.Match(m => m
                                .Field(p => p.name)
                                .Query(keyword)
                                .Operator(Operator.And)
                                .Boost(5)
                            ),
                            // Tìm kiếm mờ
                            sh => sh.Match(m => m
                                .Field(p => p.name)
                                .Query(keyword)
                                .Fuzziness(Fuzziness.Auto)
                                .Operator(Operator.Or)
                                .Boost(3)
                            ),
                            // Tìm kiếm trong các trường khác
                            sh => sh.MultiMatch(mm => mm
                                .Fields(f => f
                                    .Field(p => p.product_code)
                                    .Field(p => p.description, 0.5)
                                )
                                .Query(keyword)
                                .Type(TextQueryType.BestFields)
                                .Boost(2)
                            ),
                            m=> m.MultiMatch(mf => mf.Fields(f=>
                                        f.Field(p => p.name)
                                        .Field(p => p.product_code)
                                    )
                                    .Query(keyword)
                                    .Type(TextQueryType.BestFields)
                                    .Analyzer("standard") // Sử dụng analyzer chuẩn hỗ trợ Unicode
                                    .Fuzziness(Fuzziness.Auto) // Cho phép tìm gần đúng
                                    .Boost(1.5)
                            )
                        )
                    )
                )
                .Sort(so => so
                    .Descending(SortSpecialField.Score)
                )
                .Size(10)
            );

            return response.Documents.ToList();
        }


    }




}

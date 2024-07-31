using DAL.Generic;
using DAL.StoreProcedure;
using HuloToys_Service.DAL.StoreProcedure;
using HuloToys_Service.Models.Entities;
using Newtonsoft.Json;

namespace HuloToys_Service.DAL
{
    public class ArticleTagDAL : GenericService<ArticleTag>
    {
        private static DbWorker _DbWorker;
        public IConfiguration configuration;
        public ArticleTagDAL(string connection, IConfiguration _configuration) : base(connection)
        {
            _DbWorker = new DbWorker(connection, _configuration);
            configuration = _configuration;
        }
        public List<long> GetTagIDByArticleID(long articleID)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var a = _DbContext.ArticleTags.Where(s => s.ArticleId == articleID).Select(s => s.TagId);
                    if (a != null && a.Count() > 0)
                    {
                        var json = JsonConvert.SerializeObject(a.Distinct().ToList());
                        return JsonConvert.DeserializeObject<List<long>>(json);
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}

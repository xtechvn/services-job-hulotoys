using Nest;

namespace HuloToys_Service.Models.Article
{
    public class ArticleCategoryESModel
    {
        [PropertyName("id")]

        public long Id { get; set; }
        [PropertyName("categoryid")]

        public int? categoryid { get; set; }
        [PropertyName("articleid")]

        public long? articleid { get; set; }
    }
}

using HuloToys_Service.Models.Article;

namespace HuloToys_Service.Repro.IRepository
{
    public interface IArticleRepository
    {
        Task<List<ArticleFeModel>> getArticleListByCategoryId(int cate_id);
        Task<ArticleFeDetailModel> GetArticleDetailLite(long article_id);
        Task<List<ArticleRelationModel>> FindArticleByTitle(string title, int parent_cate_faq_id);

        Task<ArticleFEModelPagnition> getArticleListByCategoryIdOrderByDate(int cate_id, int skip, int take, string category_name);
  
        Task<ArticleFeModel> GetMostViewedArticle(long article_id);
     
        Task<ArticleFEModelPagnition> getArticleListByTags(string tag, int skip, int take);
      
    }
}

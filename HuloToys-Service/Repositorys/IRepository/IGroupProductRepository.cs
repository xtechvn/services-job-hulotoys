using HuloToys_Service.Models.Article;

namespace HuloToys_Service.Repro.IRepository
{
    public interface IGroupProductRepository
    {
        public Task<string> GetGroupProductNameAsync(int cateID);
        public Task<List<ArticleGroupViewModel>> GetArticleCategoryByParentID(long parent_id);
        public Task<List<ArticleGroupViewModel>> GetFooterCategoryByParentID(long parent_id);
        public Task<List<ProductGroupViewModel>> GetProductGroupByParentID(long parent_id, string url_static);
    }
}

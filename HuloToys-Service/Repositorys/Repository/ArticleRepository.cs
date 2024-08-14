using Entities.ConfigModels;
using HuloToys_Service.ElasticSearch.DAL;
using HuloToys_Service.Models.Article;

using HuloToys_Service.Repro.IRepository;
using HuloToys_Service.Utilities.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Repro.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ArticleDAL _ArticleDAL;
        private readonly TagDAL _TagDAL;
        private readonly GroupProductDAL _groupProductDAL;
        private readonly IConfiguration configuration;
        private readonly string _UrlStaticImage;
        public ArticleRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig, IConfiguration _configuration)
        {
            var _StrConnection = "";
            _UrlStaticImage = domainConfig.Value.ImageStatic;
            _ArticleDAL = new ArticleDAL(_StrConnection, _configuration);
            _TagDAL = new TagDAL(_StrConnection, _configuration);
            _groupProductDAL = new GroupProductDAL(_StrConnection, _configuration);
            configuration = _configuration;
        }

        // Lấy ra danh sách các bài viết theo 1 chuyên mục
        public async Task<List<ArticleFeModel>> getArticleListByCategoryId(int cate_id)
        {
            try
            {
                return await _ArticleDAL.getArticleListByCategoryId(cate_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]ArticleRepository - GetArticleDetail: " + ex);
                return null;
            }
        }

        // Lấy ra chi tiết bài viết
        public async Task<ArticleFeDetailModel> GetArticleDetailLite(long article_id)
        {
            try
            {
                return await _ArticleDAL.GetArticleDetailLite(article_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]ArticleRepository - GetArticleDetail: " + ex);
                return null;
            }
        }
    
        public async Task<ArticleFeModel> GetMostViewedArticle(long article_id)
        {
            try
            {
                var detail = await _ArticleDAL.GetArticleDetailLite(article_id);

                if (detail != null)
                {
                    var group = _groupProductDAL.GetById(detail.category_id);
                    if (!group.IsShowHeader) return null;
                    var fe_detail = new ArticleFeModel()
                    {
                        id = detail.id,
                        lead = detail.lead,
                        publish_date = detail.publish_date,
                        title = detail.title,
                        image_11 = detail.image_11,
                        image_43 = detail.image_43,
                        image_169 = detail.image_169,
                        article_type = detail.article_type,
                        category_name = detail.category_name,

                    };
                    return fe_detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]ArticleRepository - GetMostViewedArticle: " + ex);
            }
            return null;
        }
        public async Task<List<ArticleRelationModel>> FindArticleByTitle(string title, int parent_cate_faq_id)
        {
            try
            {
                return await _ArticleDAL.FindArticleByTitle(title, parent_cate_faq_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]ArticleRepository - FindArticleByTitle: " + ex);

                return null;
            }
        }

       
        public async Task<ArticleFEModelPagnition> getArticleListByCategoryIdOrderByDate(int cate_id, int skip, int take, string category_name)
        {
            try
            {
                return await _ArticleDAL.getArticleListByCategoryIdOrderByDate(cate_id, skip, take, category_name);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]getArticleListByCategoryIdOrderByDate - GetArticleDetail: " + ex);
                return null;
            }
        }
       
        public async Task<ArticleFEModelPagnition> getArticleListByTags(string tag, int skip, int take)
        {
            try
            {
                return await _ArticleDAL.GetArticleListByTag(tag, skip, take);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]getArticleListByTags - GetArticleDetail: " + ex);
                return null;
            }
        }

    }
}

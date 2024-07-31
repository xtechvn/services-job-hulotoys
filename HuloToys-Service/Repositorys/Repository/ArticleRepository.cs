﻿using Entities.ConfigModels;
using HuloToys_Service.DAL;
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
            var _StrConnection = dataBaseConfig.Value.SqlServer.ConnectionString;
            _UrlStaticImage = domainConfig.Value.ImageStatic;
            _ArticleDAL = new ArticleDAL(_StrConnection, _configuration);
            _TagDAL = new TagDAL(_StrConnection, _configuration);
            _groupProductDAL = new GroupProductDAL(_StrConnection, _configuration);
            configuration = _configuration;
        }

        public async Task<long> ChangeArticleStatus(long Id, int Status)
        {
            try
            {
                var model = await _ArticleDAL.FindAsync(Id);
                model.Status = Status;

                if (Status == ArticleStatus.PUBLISH)
                {
                    model.PublishDate = DateTime.Now;
                }

                await _ArticleDAL.UpdateAsync(model);
                return Id;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<List<int>> GetArticleCategoryIdList(long Id)
        {
            return await _ArticleDAL.GetArticleCategoryIdList(Id);
        }

        public async Task<ArticleModel> GetArticleDetail(long Id)
        {
            try
            {
                var model = await _ArticleDAL.GetArticleDetail(Id);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API] ArticleRepository - GetArticleDetail: " + ex);
                
                return null;
            }
        }

        public GenericViewModel<ArticleViewModel> GetPagingList(ArticleSearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<ArticleViewModel>();
            try
            {
                DataTable dt = _ArticleDAL.GetPagingList(searchModel, currentPage, pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = (from row in dt.AsEnumerable()
                                      select new ArticleViewModel
                                      {
                                          Id = Convert.ToInt64(row["Id"]),
                                          Title = row["Title"].ToString(),
                                          Image169 = row["Image169"].ToString(),

                                          PublishDate = Convert.ToDateTime(!row["PublishDate"].Equals(DBNull.Value) ? row["PublishDate"] : DateTime.MinValue),
                                          ModifiedOn = Convert.ToDateTime(!row["ModifiedOn"].Equals(DBNull.Value) ? row["ModifiedOn"] : DateTime.MinValue),
                                          Status = Convert.ToInt32(!row["Status"].Equals(DBNull.Value) ? row["Status"] : 0),
                                          PageView = Convert.ToInt32(!row["PageView"].Equals(DBNull.Value) ? row["PageView"] : 0),

                                          AuthorId = Convert.ToInt32(!row["AuthorId"].Equals(DBNull.Value) ? row["AuthorId"] : 0),
                                          AuthorName = row["AuthorName"].ToString(),
                                          ArticleStatusName = row["ArticleStatusName"].ToString(),
                                          ArticleCategoryName = row["ArticleCategoryName"].ToString()

                                      }).ToList();

                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / pageSize);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetPagingList - GetArticleDetail: " + ex);

            }
            return model;
        }

        public Task<List<string>> GetSuggestionTag(string name)
        {
            return _TagDAL.GetSuggestionTag(name);
        }

        public async Task<long> SaveArticle(ArticleModel model, string url_upload, string key_encrypt)
        {
            try
            {
                #region upload image
                // upload image inside content to static site
                var TBody = StringHelpers.ReplaceEditorImage(model.Body, _UrlStaticImage, url_upload, key_encrypt);

                // upload thumb image via api
                var T11 = UpLoadHelper.UploadBase64Src(model.Image11, _UrlStaticImage, url_upload, key_encrypt);
                var T43 = UpLoadHelper.UploadBase64Src(model.Image43, _UrlStaticImage, url_upload, key_encrypt);
                var T169 = UpLoadHelper.UploadBase64Src(model.Image169, _UrlStaticImage, url_upload, key_encrypt);

                await Task.WhenAll(TBody, T11, T43, T169);

                model.Body = TBody.Result;
                model.Image11 = T11.Result;
                model.Image43 = T43.Result;
                model.Image169 = T169.Result;

                if (!string.IsNullOrEmpty(model.Image11) && !model.Image11.Contains(_UrlStaticImage))
                {
                    model.Image11 = _UrlStaticImage + model.Image11;
                }
                if (!string.IsNullOrEmpty(model.Image43) && !model.Image43.Contains(_UrlStaticImage))
                {
                    model.Image43 = _UrlStaticImage + model.Image43;
                }
                if (!string.IsNullOrEmpty(model.Image169) && !model.Image169.Contains(_UrlStaticImage))
                {
                    model.Image169 = _UrlStaticImage + model.Image169;
                }
                #endregion

                #region date
                if (model.Status == ArticleStatus.PUBLISH && model.PublishDate == DateTime.MinValue)
                    model.PublishDate = DateTime.Now;

                if (model.PublishDate != DateTime.MinValue && model.DownTime == DateTime.MinValue)
                    model.DownTime = model.PublishDate.AddHours(1);
                #endregion

                var ArticleId = await _ArticleDAL.SaveArticle(model);

                if (ArticleId > 0)
                {
                    #region upsert Tags
                    var ListTagId = await _TagDAL.MultipleInsertTag(model.Tags);
                    await _ArticleDAL.MultipleInsertArticleTag(ArticleId, ListTagId);
                    #endregion

                    #region upsert Categories
                    await _ArticleDAL.MultipleInsertArticleCategory(ArticleId, model.Categories);
                    #endregion

                    #region upsert Relation Article
                    await _ArticleDAL.MultipleInsertArticleRelation(ArticleId, model.RelatedArticleIds);
                    #endregion
                }

                return ArticleId;
            }
            catch
            {
                return 0;
            }
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
        public async Task<ArticleFeModel> GetArticleDetailLiteFE(long article_id)
        {
            try
            {
                var detail = await _ArticleDAL.GetArticleDetailLite(article_id);
                if (detail != null)
                {
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
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]ArticleRepository - GetArticleDetail: " + ex);
            }
            return null;
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

        public async Task<long> DeleteArticle(long Id)
        {
            return await _ArticleDAL.DeleteArticle(Id);
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
        public async Task<ArticleFEModelPagnition> getFooterArticleListByCategory(int cate_id, int skip, int take, string category_name)
        {
            try
            {
                return await _ArticleDAL.getFooterArticleListByCategory(cate_id, skip, take, category_name);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[API]getFooterArticleListByCategory - GetArticleDetail: " + ex);
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
        public async Task<ArticleFeModel> GetPinnedArticleByposition(int cate_id, string category_name, int position)
        {
            return await _ArticleDAL.getPinnedArticleByposition(cate_id, category_name, position);
        }

    }
}

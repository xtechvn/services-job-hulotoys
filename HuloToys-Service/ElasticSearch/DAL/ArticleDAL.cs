using HuloToys_Service.ElasticSearch.NewEs;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Models.Entities;
using HuloToys_Service.Utilities.Lib;
using System.Data;
using Utilities.Contants;

namespace HuloToys_Service.ElasticSearch.DAL
{
    public class ArticleDAL
    {

        public IConfiguration configuration;
        public ArticleESService articleESService;
        public ArticleTagESService articleTagESService;
        public TagESService tagESService;
        public ArticleCategoryESService articleCategoryESService;
        public ArticleRelatedESService articleRelatedESService;
        public GroupProductESService groupProductESService;
        public ArticleDAL(string connection, IConfiguration _configuration)
        {

            configuration = _configuration;
            articleESService = new ArticleESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            articleTagESService = new ArticleTagESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            tagESService = new TagESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            articleCategoryESService = new ArticleCategoryESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            articleRelatedESService = new ArticleRelatedESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            groupProductESService = new GroupProductESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }



        public async Task<ArticleModel> GetArticleDetail(long Id)
        {
            try
            {
                var model = new ArticleModel();

                try
                {
                    var article = articleESService.GetDetailById(Id);
                    model = new ArticleModel
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Lead = article.Lead,
                        Body = article.Body,
                        Status = article.Status,
                        ArticleType = article.ArticleType,
                        Image11 = article.Image11,
                        Image43 = article.Image43,
                        Image169 = article.Image169,
                        PublishDate = article.PublishDate ?? DateTime.MinValue,
                        DownTime = article.DownTime ?? DateTime.MinValue,
                        Position = article.Position ?? 0
                    };
                    var data_ArticleTag_By_ArticleId = articleTagESService.GetListArticleTagByArticleId(article.Id);
                    var TagIds = data_ArticleTag_By_ArticleId.Select(s => s.TagId).ToList();
                    var List_tag = tagESService.GetListTag();
                    var List_articleCategory = articleCategoryESService.GetByArticleId(article.Id);
                    var List_relatedArticleIds = articleRelatedESService.GetListArticleRelatedByArticleId(article.Id);
                    model.Tags = List_tag.Where(s => TagIds.Contains(s.Id)).Select(s => s.TagName).ToList();
                    model.Categories = List_articleCategory.Select(s => (int)s.CategoryId).ToList();
                    model.MainCategory = model.Categories != null || model.Categories.Count > 0 ? model.Categories[0] : -1;
                    model.RelatedArticleIds = List_relatedArticleIds.Select(s => (long)s.ArticleRelatedId).ToList();

                    if (model.RelatedArticleIds != null && model.RelatedArticleIds.Count > 0)
                    {
                        foreach (var item in model.RelatedArticleIds)
                        {
                            var groupProductName = string.Empty;
                            var detail_article = articleESService.GetDetailById(item);
                            var articleCategory = articleCategoryESService.GetByArticleId(detail_article.Id);
                            if (articleCategory != null && articleCategory.Count > 0)
                            {
                                foreach (var item2 in articleCategory)
                                {
                                    var groupProduct = groupProductESService.GetDetailGroupProductById((long)item2.CategoryId);
                                    groupProductName += groupProduct.Name + ",";
                                }
                            }

                            var ArticleRelation = new ArticleRelationModel
                            {
                                Id = detail_article.Id,
                                Image = detail_article.Image169 ?? detail_article.Image43 ?? detail_article.Image11,
                                Title = detail_article.Title,
                                publish_date = detail_article.PublishDate ?? DateTime.Now,
                                category_name = groupProductName ?? "Tin tức"
                            };
                            model.RelatedArticleList.Add(ArticleRelation);
                        }
                        model.RelatedArticleList = model.RelatedArticleList.GroupBy(x => x.Id).Select(x => x.First()).ToList();
                    }


                }
                catch
                {

                    return null;
                }

                return model;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// cuonglv
        /// Lấy ra danh sách các bài thuộc 1 chuyên mục
        /// </summary>
        /// <param name="cate_id"></param>
        /// <returns></returns>
        public async Task<List<ArticleFeModel>> getArticleListByCategoryId(int cate_id)
        {
            try
            {
                var list_article = new List<ArticleFeModel>();

                try
                {
                    var List_articleCategory = articleCategoryESService.GetByCategoryId(cate_id);
                    if (List_articleCategory != null && List_articleCategory.Count > 0)
                    {
                        foreach (var item in List_articleCategory)
                        {
                            var _article = articleESService.GetDetailById((long)item.ArticleId);
                            if (_article == null) break;
                            var detail_model = new ArticleFeModel
                            {
                                id = _article.Id,
                                title = _article.Title,
                                lead = _article.Lead,
                                image_169 = _article.Image169,
                                image_43 = _article.Image43,
                                image_11 = _article.Image11,
                                publish_date = (DateTime)_article.PublishDate,
                                body = _article.Body
                            };
                            if (_article.Status == ArticleStatus.PUBLISH)
                                list_article.Add(detail_model);
                        }
                    }
                    list_article = list_article.Where(x => x.body != null && x.body.Trim() != "" && x.lead != null && x.lead.Trim() != "" && x.title != null && x.title.Trim() != "").ToList();

                    return list_article;
                }
                catch
                {

                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "getArticleListByCategoryId - ArticleDAL: " + ex);
                return null;
            }
        }

        /// <summary>
        /// cuonglv
        /// Lấy ra chi tiết bài viết
        /// </summary>
        /// <param name="article_id"></param>
        /// <returns></returns>
        public async Task<ArticleFeDetailModel> GetArticleDetailLite(long article_id)
        {
            try
            {
                var model = new ArticleFeDetailModel();

                var article = articleESService.GetDetailById(article_id);
                if (article == null) return null;
                model = new ArticleFeDetailModel
                {
                    id = article.Id,
                    title = article.Title,
                    lead = article.Lead,
                    body = article.Body,
                    status = article.Status,
                    article_type = article.ArticleType,
                    image_11 = article.Image11,
                    image_43 = article.Image43,
                    image_169 = article.Image169,
                    publish_date = article.PublishDate ?? DateTime.Now,
                    author_id = (int)article.AuthorId
                };

                var data_ArticleTag_By_ArticleId = articleTagESService.GetListArticleTagByArticleId(article.Id);
                var TagIds = data_ArticleTag_By_ArticleId.Select(s => s.TagId).ToList();
                var List_tag = tagESService.GetListTag();
                var List_articleCategory = articleCategoryESService.GetByArticleId(article.Id);
                var List_relatedArticleIds = articleRelatedESService.GetListArticleRelatedByArticleId(article.Id);



                model.Categories = List_articleCategory.Select(s => (int)s.CategoryId).ToList();
                model.category_id = model.Categories != null || model.Categories.Count > 0 ? model.Categories[0] : -1;
                model.MainCategory = model.Categories != null || model.Categories.Count > 0 ? model.Categories[0] : -1;
                var group_product = groupProductESService.GetDetailGroupProductById(model.MainCategory);
                if (group_product == null || group_product.Id < 0) return null;

                if (model.MainCategory > 0)
                {
                    model.category_name = group_product.Name;
                }
                else
                {
                    model.category_name = "Tin tức";
                }
                model.RelatedArticleIds = List_relatedArticleIds != null ? List_relatedArticleIds.Select(s => (long)s.ArticleRelatedId).ToList() : null;

                if (model.RelatedArticleIds != null && model.RelatedArticleIds.Count > 0)
                {
                    foreach (var item in model.RelatedArticleIds)
                    {
                        var groupProductName = string.Empty;
                        var detail_article = articleESService.GetDetailById(item);
                        var articleCategory = articleCategoryESService.GetByArticleId(detail_article.Id);
                        if (articleCategory != null && articleCategory.Count > 0)
                        {
                            foreach (var item2 in articleCategory)
                            {
                                var groupProduct = groupProductESService.GetDetailGroupProductById((long)item2.CategoryId);
                                groupProductName += groupProduct.Name + ",";
                            }
                        }

                        var ArticleRelation = new ArticleRelationModel
                        {
                            Id = detail_article.Id,
                            Image = detail_article.Image169 ?? detail_article.Image43 ?? detail_article.Image11,
                            Title = detail_article.Title,
                            publish_date = detail_article.PublishDate ?? DateTime.Now,
                            category_name = groupProductName ?? "Tin tức"
                        };
                        model.RelatedArticleList.Add(ArticleRelation);
                    }
                    model.RelatedArticleList = model.RelatedArticleList.GroupBy(x => x.Id).Select(x => x.First()).ToList();
                }

                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[article_id = " + article_id + "]GetArticleDetailLite - ArticleDAL: " + ex);
                return null;
            }
        }

        /// <summary>
        /// cuonglv
        /// Lọc những bài faq theo title
        /// </summary>
        /// <param name="article_id"></param>
        /// <returns></returns>
        public async Task<List<ArticleRelationModel>> FindArticleByTitle(string title, int parent_cate_faq_id)
        {
            try
            {
                var list_article = new List<ArticleRelationModel>();

                try
                {
                    var arr_cate_child_help_id = new List<int>();
                    var group_product_detail = groupProductESService.GetDetailGroupProductById(parent_cate_faq_id);
                    if (group_product_detail == null)
                    {
                        var group_product_list = groupProductESService.GetListGroupProductByParentId(parent_cate_faq_id);
                        arr_cate_child_help_id = group_product_list.Select(x => x.Id).ToList();
                    }
                    arr_cate_child_help_id.Add(group_product_detail.Id);



                    if (arr_cate_child_help_id.Count() > 0)
                    {
                        foreach (var item in arr_cate_child_help_id)
                        {
                            var groupProductName = string.Empty;
                            var DetailGroupProductById = groupProductESService.GetDetailGroupProductById(item);
                            if (DetailGroupProductById.IsShowHeader == true)
                            {
                                groupProductName += DetailGroupProductById.Name + ",";
                            }
                            var List_articleCategory = articleCategoryESService.GetByCategoryId(DetailGroupProductById.Id);
                            if (List_articleCategory != null && List_articleCategory.Count > 0)
                            {
                                foreach (var item2 in List_articleCategory)
                                {
                                    var detail_article = articleESService.GetDetailById((long)item2.ArticleId);
                                    if (detail_article != null)
                                    {
                                        var ArticleRelation = new ArticleRelationModel
                                        {
                                            Id = detail_article.Id,
                                            Lead = detail_article.Lead,
                                            Image = detail_article.Image169 ?? detail_article.Image43 ?? detail_article.Image11,
                                            Title = detail_article.Title,
                                            publish_date = detail_article.PublishDate ?? DateTime.Now,
                                            category_name = groupProductName ?? "Tin tức"
                                        };
                                        list_article.Add(ArticleRelation);
                                    }

                                }
                            }

                        }
                        if (list_article.Count > 0)
                            list_article = list_article.Where(s => s.Title.ToUpper().Contains(title.ToUpper())).GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.publish_date).ToList();

                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception ex)
                {

                    LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[title = " + title + "]FindArticleByTitle - ArticleDAL: transaction.Commit " + ex);
                    return null;
                }

                return list_article;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[title = " + title + "]FindArticleByTitle - ArticleDAL:" + ex);

                return null;
            }
        }

        /// <summary>
        /// minh.nq
        /// Lấy ra danh sách các bài thuộc 1 chuyên mục, phân trang+ sắp xếp theo ngày mới nhất
        /// </summary>
        /// <param name="cate_id"></param>
        /// <returns></returns>
        public async Task<ArticleFEModelPagnition> getArticleListByCategoryIdOrderByDate(int cate_id, int skip, int take, string category_name)
        {
            try
            {

                var list_postion_pinned = new List<short?> { 1, 2, 3 };

                try
                {
                    var list_article = new List<ArticleFeModel>();
                    var list_article2 = new List<ArticleFeModel>();
                    var list_pinned = new List<ArticleFeModel>();
                    var data = articleCategoryESService.GetByCategoryId(cate_id);
                    if (data != null && data.Count > 0)
                    {
                        data = data.GroupBy(s => s.ArticleId).Select(s => s.First()).ToList();
                        data = data.Where(s => s.CategoryId == cate_id).ToList();
                        foreach (var item in data)
                        {
                            var groupProductName = string.Empty;
                            var groupProductId = string.Empty;
                            var article_Category = articleCategoryESService.GetByArticleId((long)item.ArticleId);
                            if (article_Category != null)
                            {
                                article_Category = article_Category.GroupBy(s => s.CategoryId).Select(s => s.First()).ToList();
                                foreach (var item2 in article_Category)
                                {
                                    var groupProduct = groupProductESService.GetDetailGroupProductById((long)item2.CategoryId);
                                    if (groupProduct != null && groupProduct.ParentId > 0)
                                    {
                                        groupProductName += groupProduct.Name + ",";
                                        groupProductId += groupProduct.Id + ",";
                                    }
                                }
                            }

                            var _article = articleESService.GetDetailById((long)item.ArticleId);
                            if (_article != null && _article.Status == ArticleStatus.PUBLISH)
                            {
                                var model = new ArticleFeModel
                                {
                                    id = _article.Id,
                                    category_name = groupProductName,
                                    title = _article.Title,
                                    lead = _article.Lead,
                                    image_169 = _article.Image169,
                                    image_43 = _article.Image43,
                                    image_11 = _article.Image11,
                                    publish_date = (DateTime)_article.PublishDate,
                                    article_type = _article.ArticleType,
                                    update_last = (DateTime)_article.ModifiedOn,
                                    position = _article.Position,
                                    category_id = groupProductId,
                                };
                                list_article.Add(model);
                            }

                        }
                    }


                    var article = articleESService.GetListArticlePosition();
                    article = article.Where(S => S.Status == ArticleStatus.PUBLISH).ToList();
                    if (article != null && article.Count > 0)
                    {
                        foreach (var _article in article)
                        {
                            var groupProductName = string.Empty;
                            var groupProductId = string.Empty;
                            var article_Category = articleCategoryESService.GetByArticleId(_article.Id);
                            if (article_Category != null)
                            {
                                article_Category = article_Category.GroupBy(s => s.CategoryId).Select(s => s.First()).ToList();
                                foreach (var item2 in article_Category)
                                {
                                    var groupProduct = groupProductESService.GetDetailGroupProductById((long)item2.CategoryId);
                                    if (groupProduct != null && groupProduct.ParentId > 0)
                                    {
                                        groupProductName += groupProduct.Name + ",";
                                        groupProductId += groupProduct.Id + ",";
                                    }
                                    else
                                    {
                                        groupProductId += groupProduct.Id + ",";
                                    }

                                }
                            }
                            var model = new ArticleFeModel
                            {
                                id = _article.Id,
                                category_name = groupProductName,
                                title = _article.Title,
                                lead = _article.Lead,
                                image_169 = _article.Image169,
                                image_43 = _article.Image43,
                                image_11 = _article.Image11,
                                publish_date = (DateTime)_article.PublishDate,
                                position = _article.Position,
                                article_type = _article.ArticleType,
                                update_last = (DateTime)_article.ModifiedOn,
                                category_id = groupProductId,
                            };

                            if (groupProductId.Contains(cate_id.ToString()))
                                list_pinned.Add(model);

                        }
                    }

                    var result = new ArticleFEModelPagnition();
                    list_article = list_article.OrderByDescending(x => x.publish_date).GroupBy(gr => gr.id).Select(x => x.First()).ToList();
                    list_pinned = list_pinned.OrderByDescending(x => x.update_last).GroupBy(gr => gr.position).Select(x => x.First()).ToList();

                    foreach (var pinned in list_pinned)
                    {

                        if (pinned.position != null && pinned.position > 0)
                        {
                            list_article.RemoveAll(x => x.title == pinned.title && x.lead == pinned.lead);
                            //list_article.Insert(((int)pinned.position - 1), pinned);
                        }
                    }
                    if (list_pinned == null || list_pinned.Count < 3)
                    {

                        foreach (var item in list_article)
                        {
                            list_pinned.Add(item);
                            list_article2.Add(item);
                            if (list_pinned.Count == 3) break;
                        }
                        foreach (var item in list_article2)
                        {

                            list_article.Remove(item);
                        }
                        foreach (var pinned in list_pinned)
                        {
                            if (list_postion_pinned.Contains(pinned.position))
                            {
                                list_postion_pinned.Remove(pinned.position);
                            }
                            else
                            {
                                if (list_postion_pinned.Count > 0)
                                    pinned.position = list_postion_pinned[0];
                                list_postion_pinned.Remove(list_postion_pinned[0]);
                            }
                        }
                    }


                    result.list_article_fe = list_article.Skip(skip).Take(take).ToList();
                    result.list_article_pinned = list_pinned;
                    result.total_item_count = list_article.Count;
                    return result;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[getArticleListByCategoryId - ArticleDAL:" + ex);
                return null;
            }
        }
        /// <summary>
        /// minh.nq
        /// Lấy ra danh sách các bài thuộc 1 chuyên mục, phân trang+ sắp xếp theo ngày mới nhất
        /// </summary>
        /// <param name="cate_id"></param>
        /// <returns></returns>
        public async Task<ArticleFEModelPagnition> GetArticleListByTag(string tag, int skip, int take)
        {
            try
            {


                var list_article = new List<ArticleFeModel>();
                var ListTag = tagESService.GetListTagByTagName(tag);
                if (ListTag != null && ListTag.Count > 0)
                {
                    foreach (var item in ListTag)
                    {
                        var articleTag = articleTagESService.GetListArticleTagByTagid(item.Id);
                        if (articleTag != null && articleTag.Count > 0)
                        {
                            articleTag = articleTag.GroupBy(s => s.ArticleId).Select(s => s.First()).ToList();
                            foreach (var item2 in articleTag)
                            {
                                var _article = articleESService.GetDetailById((long)item2.ArticleId);
                                var model = new ArticleFeModel
                                {
                                    id = _article.Id,
                                    category_name = "",
                                    title = _article.Title,
                                    lead = _article.Lead,
                                    image_169 = _article.Image169,
                                    image_43 = _article.Image43,
                                    image_11 = _article.Image11,
                                    publish_date = (DateTime)_article.PublishDate,
                                    article_type = _article.ArticleType,
                                    position = _article.Position
                                };
                                list_article.Add(model);
                            }
                        }
                    }

                }


                var result = new ArticleFEModelPagnition();
                result.list_article_fe = list_article.Skip(skip).Take(take).ToList();
                result.total_item_count = list_article.Count;
                return result;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "[getArticleListByCategoryId - ArticleDAL:" + ex);
                return null;
            }
        }

    }
}

using ENTITIES.ViewModels.ArticleViewModels;
using HuloToys_Service.Utilities.Lib;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace HuloToys_Service.Models.MongoDb
{
    public class NewsMongoService
    {
        private IMongoCollection<NewsViewCount> newsmongoCollection;
        private IConfiguration Configuration;
        public NewsMongoService(IConfiguration _Configuration)
        {
            Configuration = _Configuration;
            string url = "mongodb://" + _Configuration["MongoServer:user"] + ":" + _Configuration["MongoServer:pwd"] + "@" + _Configuration["MongoServer:Host"] + ":" + _Configuration["MongoServer:Port"] + "/" + _Configuration["MongoServer:catalog_core"];
            var client = new MongoClient(url);
            IMongoDatabase db = client.GetDatabase(_Configuration["MongoServer:catalog_core"]);
            newsmongoCollection = db.GetCollection<NewsViewCount>("ArticlePageView");

        }
        public async Task<string> AddNewOrReplace(NewsViewCount model)
        {
            try
            {
                var filter = Builders<NewsViewCount>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<NewsViewCount>.Filter.Eq(x => x.articleID, model.articleID);
                var exists_model = await newsmongoCollection.Find(filterDefinition).FirstOrDefaultAsync();
                if (exists_model != null && exists_model.articleID == model.articleID)
                {
                    exists_model.pageview = exists_model.pageview + model.pageview;
                    await newsmongoCollection.FindOneAndReplaceAsync(filterDefinition, exists_model);
                    return exists_model._id;
                }
                else
                {
                    model.GenID();
                    await newsmongoCollection.InsertOneAsync(model);
                    return model._id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(Configuration["telegram:log_try_catch:bot_token"], Configuration["telegram:log_try_catch:group_id"], "AddNewOrReplace - NewsMongoService: " + ex);
                return null;
            }
        }
        public async Task<List<NewsViewCount>> GetMostViewedArticle()
        {
            try
            {
                var filter = Builders<NewsViewCount>.Filter;
                var filterDefinition = filter.Empty;
                var list = await newsmongoCollection.Find(filterDefinition).SortByDescending(x => x.pageview).ToListAsync();
                if (list != null && list.Count > 0)
                {
                    if (list.Count < 10) return list;
                    else return list.Skip(0).Take(10).ToList();
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(Configuration["telegram:log_try_catch:bot_token"], Configuration["telegram:log_try_catch:group_id"], "GetMostViewedArticle - NewsMongoService: " + ex);
            }
            return null;
        }

    }
}

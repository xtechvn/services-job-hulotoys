﻿using Entities.ConfigModels;
using HuloToys_Service.DAL;
using HuloToys_Service.Repro.IRepository;
using Microsoft.Extensions.Options;

namespace HuloToys_Service.Repro.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ArticleTagDAL articleTagDAL;
        private readonly TagDAL _tagDAL;
        public IConfiguration configuration;

        public TagRepository(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration _configuration)
        {
            articleTagDAL = new ArticleTagDAL(dataBaseConfig.Value.SqlServer.ConnectionString,_configuration);
            _tagDAL = new TagDAL(dataBaseConfig.Value.SqlServer.ConnectionString,_configuration);
            configuration = _configuration;
        }
        public async Task<List<string>> GetAllTagByArticleID(long articleID)
        {
            var tag_id_list = articleTagDAL.GetTagIDByArticleID(articleID);
            return await _tagDAL.GetTagByListID(tag_id_list);
        }
    }
}

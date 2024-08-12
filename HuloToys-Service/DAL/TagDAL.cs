﻿using Amazon.SecurityToken.Model;
using DAL.Generic;
using DAL.StoreProcedure;
using HuloToys_Service.DAL.StoreProcedure;
using HuloToys_Service.ElasticSearch.NewEs;
using HuloToys_Service.Utilities.Lib;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HuloToys_Service.DAL
{
    public class TagDAL : GenericService<Tag>
    {
        private static DbWorker _DbWorker;
        private readonly IConfiguration configuration;
        public TagESService tagESService;
        public TagDAL(string connection, IConfiguration _configuration) : base(connection)
        {
            _DbWorker = new DbWorker(connection, _configuration);
            configuration = _configuration;
            tagESService = new TagESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }

        public async Task<List<string>> GetTagByListID(List<long> tag_id_list)
        {
            try
            {
                var list_name = new List<string>();
               if(tag_id_list!= null && tag_id_list.Count > 0)
                {
                    foreach(var item in tag_id_list)
                    {
                        var tag = tagESService.GetListTagById(item);
                        if(tag !=null) { list_name.Add(tag.TagName); }
                    }
                }
                    return list_name;
                
            }
            catch(Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return null;
            }
        }
    }
}

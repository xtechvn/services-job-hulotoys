using Amazon.SecurityToken.Model;
using DAL.Generic;
using DAL.StoreProcedure;
using HuloToys_Service.DAL.StoreProcedure;
using Microsoft.EntityFrameworkCore;

namespace HuloToys_Service.DAL
{
    public class TagDAL : GenericService<Tag>
    {
        private static DbWorker _DbWorker;
        private readonly IConfiguration configuration;
        public TagDAL(string connection, IConfiguration _configuration) : base(connection)
        {
            _DbWorker = new DbWorker(connection, _configuration);
            configuration = _configuration;
        }
        public async Task<List<long>> MultipleInsertTag(List<string> TagList)
        {
            var ListResult = new List<long>();
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    using (var transaction = _DbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            if (TagList != null && TagList.Count >= 0)
                            {
                                foreach (var item in TagList)
                                {
                                    var tagItemModel = await _DbContext.Tags.FirstOrDefaultAsync(s => s.TagName == item.Trim());
                                    if (tagItemModel == null)
                                    {
                                        var tagModel = new Models.Entities.Tag()
                                        {
                                            TagName = item,
                                            CreatedOn = DateTime.Now
                                        };
                                        await _DbContext.Tags.AddAsync(tagModel);
                                        await _DbContext.SaveChangesAsync();
                                        ListResult.Add(tagModel.Id);
                                    }
                                    else
                                    {
                                        ListResult.Add(tagItemModel.Id);
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return null;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return ListResult;
        }

        public async Task<List<string>> GetSuggestionTag(string name)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Tags.Where(s => s.TagName.Trim().ToLower().Contains(name.ToLower())).Select(s => s.TagName).Take(10).ToListAsync();
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<string>> GetTagByListID(List<long> tag_id_list)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Tags.Where(s => tag_id_list.Contains(s.Id)).Select(s => s.TagName).ToListAsync();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

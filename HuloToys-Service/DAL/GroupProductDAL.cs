using DAL.Generic;
using HuloToys_Service.DAL.StoreProcedure;
using HuloToys_Service.Models.Entities;
using HuloToys_Service.Utilities.Lib;
using Utilities.Contants;

namespace HuloToys_Service.DAL
{
    public class GroupProductDAL : GenericService<GroupProduct>
    {
        public IConfiguration configuration;
        public GroupProductDAL(string connection, IConfiguration _configuration) : base(connection)
        {
            configuration = _configuration;
        }

        public List<GroupProduct> GetByParentId(long parent_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupProducts.Where(s => s.ParentId == parent_id && s.Status == (int)ArticleStatus.PUBLISH).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByParentId-GroupProductDAL" + ex);
            }
            return null;
        }
        public GroupProduct GetById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.GroupProducts.Where(s => s.Id == id && s.Status == (int)ArticleStatus.PUBLISH).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetById-GroupProductDAL" + ex);

            }
            return null;
        }
    }
}

using Entities.ConfigModels;
using HuloToys_Service.DAL;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Repro.IRepository;
using HuloToys_Service.Utilities.Lib;
using Microsoft.Extensions.Options;
using Utilities;

namespace HuloToys_Service.Repro.Repository
{
    public class GroupProductRepository : IGroupProductRepository
    {
        private readonly GroupProductDAL _GroupProductDAL;
        private readonly IConfiguration configuration;

        public GroupProductRepository(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration _configuration)
        {
            _GroupProductDAL = new GroupProductDAL("", _configuration);
            configuration = _configuration;
        }



        public async Task<string> GetGroupProductNameAsync(int cateID)
        {
            string group_name = null;
            try
            {
                var _groupList = await _GroupProductDAL.GetAllAsync();
                var dataModel = _groupList.Where(s => s.Id == cateID).FirstOrDefault();
                if (dataModel == null || dataModel.Name == null) return "";
                group_name = dataModel.Name;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetGroupProductNameAsync -GroupProductRepository : " + ex);
            }
            return group_name;
        }

        public async Task<List<ArticleGroupViewModel>> GetArticleCategoryByParentID(long parent_id)
        {
            try
            {
                var group = _GroupProductDAL.GetByParentId(parent_id);
                group = group.Where(x => x.IsShowHeader == true).ToList();
                var list = new List<ArticleGroupViewModel>();
                //list.Add(new ArticleGroupViewModel()
                //{
                //    id = parent_id,
                //    name = "Mới nhất",
                //    order_no = -1,
                //    image_path = "",
                //    url_path = "tin-tuc-" + parent_id
                //});
                list.AddRange(group.Select(x => new ArticleGroupViewModel() { id = x.Id, image_path = x.ImagePath, name = x.Name, order_no = (int)x.OrderNo, url_path = x.Path }).OrderBy(x => x.order_no).ToList());
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetArticleCategoryByParentID -GroupProductRepository : " + ex);
            }
            return null;
        }
        public async Task<List<ArticleGroupViewModel>> GetFooterCategoryByParentID(long parent_id)
        {
            try
            {
                var group = _GroupProductDAL.GetByParentId(parent_id);
                group = group.Where(x => x.IsShowFooter == true).ToList();
                var list = new List<ArticleGroupViewModel>();

                list.AddRange(group.Select(x => new ArticleGroupViewModel() { id = x.Id, image_path = x.ImagePath, name = x.Name, order_no = (int)x.OrderNo, url_path = x.Path }).OrderBy(x => x.order_no).ToList());
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetFooterCategoryByParentID -GroupProductRepository : " + ex);
            }
            return null;
        }
        public async Task<List<ProductGroupViewModel>> GetProductGroupByParentID(long parent_id, string url_static)
        {
            try
            {
                var group = _GroupProductDAL.GetByParentId(parent_id);
                var list = new List<ProductGroupViewModel>();
                list.AddRange(group.Select(x => new ProductGroupViewModel() { id = x.Id, image = url_static + x.ImagePath, name = x.Name, code = Convert.ToInt32(x.Code), link = CommonHelper.RemoveSpecialCharacters(CommonHelper.RemoveUnicode(x.Name.ToLower())).Replace(" ", "-").Replace("--", "-") }).ToList());
                return list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetProductGroupByParentID -GroupProductRepository : " + ex);
            }
            return null;
        }
    }
}

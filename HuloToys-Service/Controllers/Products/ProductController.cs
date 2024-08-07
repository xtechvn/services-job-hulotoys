using Caching.Elasticsearch;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.Products;
using HuloToys_Service.Models.Products;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.APIRequest;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly IProductESRepository<object> _ESRepository;
        private readonly ILocationProductRepository _locationProductRepository;
        private readonly RedisConn _redisService;

        public ProductController(IConfiguration _configuration, RedisConn redisService, ILocationProductRepository locationProductRepository)
        {
            configuration = _configuration;
            workQueueClient = new WorkQueueClient(configuration);
            accountClientESService = new AccountClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            clientESService = new ClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            _ESRepository = new ProductESRepository<object>(_configuration["DataBaseConfig:Elastic:Host"]);
            _redisService = redisService;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            _locationProductRepository = locationProductRepository;
        }
        [HttpPost("detail")]
        public async Task<ActionResult> getProductDetail([FromBody] APIRequestGenericModel input)
        {
            string msg = string.Empty;

            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    string id = objParr[0]["id"].ToString();
                    if (id != null && id.Trim().Length == 0)
                    {
                        var product = _ESRepository.getProductDetailById(configuration["Elastic:Index:Product"], id);
                        return Ok(new
                        {
                            status = product!=null?(int)ResponseType.SUCCESS: (int)ResponseType.FAILED,
                            msg = ResponseMessages.FunctionExcutionFailed,
                            data=product
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = ResponseMessages.FunctionExcutionFailed
                });

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });
        }
        [HttpPost("get-list")]
        public async Task<ActionResult> GetListProductByGroupId([FromBody] APIRequestGenericModel input)
        {
            string msg = string.Empty;

            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    int? group_id = Convert.ToInt32(objParr[0]["group_id"].ToString());
                    int? page_index = Convert.ToInt32(objParr[0]["page_index"].ToString());
                    if (page_index == null) page_index = 1;
                    int? page_size = Convert.ToInt32(objParr[0]["page_size"].ToString());
                    if (page_size == null) page_size = 10;

                    if (group_id != null && group_id >0)
                    {
                        string cache_name = CacheType.PRODUCT_BY_GROUP + group_id;
                        var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                        if (j_data != null) {

                            var products = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductByGroupViewModel>(j_data);    
                            if(products!=null && products.obj_lst_product_result!=null && products.obj_lst_product_result.Count > 0)
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    //msg = ResponseMessages.FunctionExcutionFailed,
                                    data = new SearchEsEntitiesViewModel()
                                    {
                                        obj_lst_product_result = products.obj_lst_product_result.Skip((int)page_index - 1).Take((int)page_size).ToList(),
                                        total_item_store = products.total_item_store
                                    }
                                });
                            }
                        }

                        var list_by_group = new List<LocationProduct>();
                        if (list_by_group==null || list_by_group.Count <= 0)
                        {
                            list_by_group = await _locationProductRepository.GetListByGroupId((int)group_id);
                        }
                        if(list_by_group!=null && list_by_group.Count > 0)
                        {
                            var products = new ProductByGroupViewModel()
                            {
                                obj_lst_product_result = new List<Entities.ViewModels.ProductViewModel>(),
                                total_item_store = 0,
                                locationProducts=list_by_group
                            };
                            foreach(var item in list_by_group)
                            {
                                products.obj_lst_product_result.Add( _ESRepository.getProductDetailByCode(configuration["Elastic:Index:Product"], item.ProductCode));
                            }
                            products.total_item_store = products.obj_lst_product_result.Count;
                            _redisService.Set(cache_name, Newtonsoft.Json.JsonConvert.SerializeObject(products), Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                //msg = ResponseMessages.FunctionExcutionFailed,
                                data = new SearchEsEntitiesViewModel()
                                {
                                    obj_lst_product_result= products.obj_lst_product_result.Skip((int)page_index-1).Take((int)page_size).ToList(),
                                    total_item_store=products.total_item_store
                                }
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = ResponseMessages.DataInvalid
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = ResponseMessages.FunctionExcutionFailed
                });

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });
        }
    }
}

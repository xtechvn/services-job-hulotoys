using Caching.Elasticsearch;
using Entities.ConfigModels;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.APIRequest;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly IProductESRepository<object> _ESRepository;
        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
            workQueueClient = new WorkQueueClient(configuration);
            accountClientESService = new AccountClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            clientESService = new ClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            _ESRepository = new ProductESRepository<object>(_configuration["DataBaseConfig:Elastic:Host"]);
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
                        var product = _ESRepository.getProductDetailByCode(configuration["Elastic:Index:Product"], id);
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
                        var product = _ESRepository.getProductListByGroupProductId(configuration["Elastic:Index:Product"], new List<int>() { (int)group_id }, (int)page_index, (int)page_size);
                        return Ok(new
                        {
                            status = product != null ? (int)ResponseType.SUCCESS : (int)ResponseType.FAILED,
                            msg = ResponseMessages.FunctionExcutionFailed,
                            data = product
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
    }
}

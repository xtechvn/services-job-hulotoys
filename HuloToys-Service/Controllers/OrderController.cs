using Caching.Elasticsearch;
using Entities.ViewModels.APIRequest;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly OrderESRepository orderESRepository;
        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;

            workQueueClient = new WorkQueueClient(configuration);
            orderESRepository = new OrderESRepository(configuration["DataBaseConfig:Elastic:Host"], configuration);
        }
     
        [HttpPost("history")]
        public async Task<ActionResult> OrderHistory(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<OrderHistoryRequestModel>(objParr[0].ToString());
                    if (request == null || request.client_id <= 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result=  orderESRepository.GetByClientID(request.client_id);

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data= result
                    });

                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });

        }
        [HttpPost("history-lastest")]
        public async Task<ActionResult> OrderHistoryLastestItem(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<OrderHistoryRequestModel>(objParr[0].ToString());
                    if (request == null|| request.client_id<=0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = orderESRepository.GetLastestClientID(request.client_id);


                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data=result
                    });

                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });

        }
        [HttpPost("history-find")]
        public async Task<ActionResult> OrderHistoryFind(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<OrderHistoryRequestModel>(objParr[0].ToString());
                    if (request == null || request.client_id <= 0 || request.order_no ==null || request.order_no.Trim()=="")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }


                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success"
                    });

                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });

        }
    }
}

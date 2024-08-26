using Caching.Elasticsearch;
using Models.APIRequest;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;
using HuloToys_Service.MongoDb;
using Models.MongoDb;
using HuloToys_Service.Models.APP;
using HuloToys_Service.Utilities.constants.APP;
using HuloToys_Service.Models.APIRequest;
using Models.Orders;
using HuloToys_Service.Models.Queue;
using HuloToys_Service.Controllers.Order.Business;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly OrderESService orderESRepository;
        private readonly OrderMongodbService orderMongodbService;
        private readonly CartMongodbService _cartMongodbService;
        private readonly WorkQueueClient work_queue;
        private readonly IdentiferService identiferService;
        private readonly QueueSettingViewModel queue_setting;
        public OrderController(IConfiguration _configuration )
        {
            configuration = _configuration;

            workQueueClient = new WorkQueueClient(configuration);
            orderESRepository = new OrderESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            orderMongodbService = new OrderMongodbService( configuration);
            _cartMongodbService = new CartMongodbService(configuration);
            work_queue = new WorkQueueClient(configuration);
            identiferService = new IdentiferService(_configuration);
            queue_setting = new QueueSettingViewModel
            {
                host = configuration["Queue:Host"],
                v_host = configuration["Queue:V_Host"],
                port = Convert.ToInt32(configuration["Queue:Port"]),
                username = configuration["Queue:Username"],
                password = configuration["Queue:Password"]
            };
        }

        [HttpPost("history")]
        public async Task<ActionResult> OrderHistory([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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
        public async Task<ActionResult> OrderHistoryLastestItem([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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
        public async Task<ActionResult> OrderHistoryFind([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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

                    var order=orderESRepository.GetByOrderNo(request.order_no,request.client_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data= order
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
        [HttpPost("detail")]
        public async Task<ActionResult> Detail([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<OrdersGeneralRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = await orderMongodbService.FindById(request.id);
                    if(result != null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = result
                        });

                    }
                    

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
        [HttpPost("confirm")]
        public async Task<ActionResult> Confirm([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<CartConfirmRequestModel>(objParr[0].ToString());
                    if (request == null || request.account_client_id == null || request.account_client_id <= 0
                        || request.carts == null || request.carts.Count <= 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var count = await orderESRepository.CountOrderByYear();
                    if (count < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.FunctionExcutionFailed
                        });
                    }
                    var order_no = await identiferService.buildOrderNo(count);
                    var model = new OrderDetailMongoDbModel()
                    {
                        account_client_id = request.account_client_id,
                        carts = new List<CartItemMongoDbModel>(),
                        payment_type = request.payment_type,
                        delivery_type = request.delivery_type,
                        order_no = order_no,
                        total_amount=0
                    };
                    
                    foreach (var item in request.carts)
                    {
                        var cart = await _cartMongodbService.FindById(item.id);
                        if (cart == null
                            || cart._id == null || cart._id.Trim() == ""
                            || cart.account_client_id != request.account_client_id)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = ResponseMessages.DataInvalid
                            });
                        }
                        else
                        {
                            cart.quanity = item.quanity;
                            model.carts.Add(cart);
                            model.total_amount += cart.total_amount;
                            await _cartMongodbService.Delete(item.id);
                        }

                    }
                    //-- Mongodb:
                    var result = await orderMongodbService.Insert(model);
                    //-- Insert Queue:
                    var queue_model = new CheckoutQueueModel() { event_id = (int)CheckoutEventID.CREATE_ORDER, order_mongo_id = result };
                    var pushed_queue=work_queue.InsertQueueSimpleDurable(queue_setting,JsonConvert.SerializeObject(queue_model) , QueueName.QUEUE_CHECKOUT);
                   
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = new OrderConfirmResponseModel { order_no = order_no, id = result, pushed= pushed_queue }
                    });
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
                msg = ResponseMessages.FunctionExcutionFailed
            });
        }
    }
}

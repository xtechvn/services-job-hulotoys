using Entities.ViewModels.APIRequest;
using Entities.ViewModels.MongoDb;
using HuloToys_Service.MongoDb;
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
    public class CartController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly CartMongodbService cartMongodbService;
        public CartController(IConfiguration _configuration)
        {
            configuration = _configuration;

            workQueueClient = new WorkQueueClient(configuration);
            cartMongodbService=new CartMongodbService(configuration);
        }
        [HttpPost("insert")]
        public async Task<ActionResult> InsertCartItem(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<CartInsertRequestModel>(objParr[0].ToString());
                    if (request == null
                        || request.client_id <= 0
                        || request.product_id <= 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }

                    CartItemMongoDbModel model = new CartItemMongoDbModel()
                    {
                        client_id = request.client_id,
                        product_id = request.product_id,
                    };
                    var result= cartMongodbService.InsertCartItem(model);
                    if (result != null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data=result
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.FunctionExcutionFailed,
                            data = result

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
        [HttpPost("count-item")]
        public async Task<ActionResult> CountCartItem(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<CartInsertRequestModel>(objParr[0].ToString());
                    if (request == null || request.client_id <= 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = await cartMongodbService.CountCartItemByClientId(request.client_id);
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
        [HttpPost("delete")]
        public async Task<ActionResult> DeleteCartItem(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<CartDeleteRequestModel>(objParr[0].ToString());
                    if (request == null || request.id ==null || request.id.Trim()=="")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = await cartMongodbService.DeleteCartItemByID(request.id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = result
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
                msg = ResponseMessages.DataInvalid
            });

        }
        [HttpPost("get")]
        public async Task<ActionResult> GetListCartItems(string token)
        {
            try
            {


                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<CartInsertRequestModel>(objParr[0].ToString());
                    if (request == null || request.client_id <= 0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = await cartMongodbService.GetCartItemByClientID(request.client_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = result
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
                msg = ResponseMessages.DataInvalid
            });

        }
        
    }
}

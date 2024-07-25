using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using HuloToys_Service.Utilities.Lib;
using Utilities;
using Utilities.Contants;
using LIB.Models.APIRequest;
using Models.Queue;
using HuloToys_Service.RabitMQ;
using Models.APIRequest;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        public ClientController(IConfiguration _configuration) {
            configuration= _configuration;

            workQueueClient=new WorkQueueClient(configuration);

        }
        [HttpPost("login")]
        public async Task<ActionResult> ClientLogin([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientLoginRequestModel>(objParr[0].ToString());
                    if (request == null 
                        || request.user_name == null || request.user_name.Trim() == ""
                        || request.password == null || request.password.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản / Mật khẩu không chính xác, vui lòng thử lại"
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

        [HttpPost("register")]
        public async Task<ActionResult> ClientRegister([FromBody] APIRequestGenericModel input)
        {
            try
            {
                

                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientRegisterRequestModel>(objParr[0].ToString());
                    if (request == null || request.user_name==null || request.user_name.Trim()==""
                        || request.phone == null || request.phone.Trim() == ""
                        || request.password == null || request.password.Trim() == ""
                        || request.confirm_password == null || request.confirm_password.Trim() == ""
                        || request.password.Trim() != request.confirm_password.Trim() ) {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }

                    AccountClientViewModel model = new AccountClientViewModel()
                    {
                        ClientId=-1,
                        ClientType=0,
                        Email=request.email==null || request.email.Trim()==""?"":request.email.Trim(),
                        Id=-1,
                        isReceiverInfoEmail=request.is_receive_email==true?(byte)1: (byte)0,
                        Name=request.user_name.Trim(),
                        Password=request.password,
                        Phone=request.phone,
                        Status=0,
                        UserName=request.user_name
                    };
                    var queue_model = new ClientConsumerQueueModel()
                    {
                        data_receiver = JsonConvert.SerializeObject(model),
                        queue_type = QueueType.ADD_USER
                    };
                    bool result= workQueueClient.InsertQueueSimple(new Models.QueueSettingViewModel()
                    {
                        host = configuration["Queue:Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        v_host = configuration["Queue:V_Host"],
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"],
                    },JsonConvert.SerializeObject(queue_model),QueueName.queue_app_push);
                    if (result)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success"
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

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientForgotPasswordRequestModel>(objParr[0].ToString());
                    if (request == null || request.name == null || request.name.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }

                    //AccountClientViewModel model = new AccountClientViewModel()
                    //{
                    //    ClientId = -1,
                    //    ClientType = 0,
                    //    Email = request.email == null || request.email.Trim() == "" ? "" : request.email.Trim(),
                    //    Id = -1,
                    //    isReceiverInfoEmail = request.is_receive_email == true ? (byte)1 : (byte)0,
                    //    Name = request.user_name.Trim(),
                    //    Password = request.password,
                    //    Phone = request.phone,
                    //    Status = 0,
                    //    UserName = request.user_name
                    //};
                    //var queue_model = new ClientConsumerQueueModel()
                    //{
                    //    data_receiver = JsonConvert.SerializeObject(model),
                    //    queue_type = QueueType.ADD_USER
                    //};
                    //bool result = workQueueClient.InsertQueueSimple(new Models.QueueSettingViewModel()
                    //{
                    //    host = configuration["Queue:Host"],
                    //    port = Convert.ToInt32(configuration["Queue:Port"]),
                    //    v_host = configuration["Queue:V_Host"],
                    //    username = configuration["Queue:Username"],
                    //    password = configuration["Queue:Password"],
                    //}, JsonConvert.SerializeObject(queue_model), QueueName.queue_app_push);
                    //if (result)
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.SUCCESS,
                    //        msg = "Success"
                    //    });
                    //}

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
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:FE"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientChangePasswordRequestModel>(objParr[0].ToString());
                    if (request == null || request.id<=0
                        || request.password == null || request.password.Trim() == ""
                        || request.confirm_password == null || request.confirm_password.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }

                    //AccountClientViewModel model = new AccountClientViewModel()
                    //{
                    //    ClientId = -1,
                    //    ClientType = 0,
                    //    Email = request.email == null || request.email.Trim() == "" ? "" : request.email.Trim(),
                    //    Id = -1,
                    //    isReceiverInfoEmail = request.is_receive_email == true ? (byte)1 : (byte)0,
                    //    Name = request.user_name.Trim(),
                    //    Password = request.password,
                    //    Phone = request.phone,
                    //    Status = 0,
                    //    UserName = request.user_name
                    //};
                    //var queue_model = new ClientConsumerQueueModel()
                    //{
                    //    data_receiver = JsonConvert.SerializeObject(model),
                    //    queue_type = QueueType.ADD_USER
                    //};
                    //bool result = workQueueClient.InsertQueueSimple(new Models.QueueSettingViewModel()
                    //{
                    //    host = configuration["Queue:Host"],
                    //    port = Convert.ToInt32(configuration["Queue:Port"]),
                    //    v_host = configuration["Queue:V_Host"],
                    //    username = configuration["Queue:Username"],
                    //    password = configuration["Queue:Password"],
                    //}, JsonConvert.SerializeObject(queue_model), QueueName.queue_app_push);
                    //if (result)
                    //{
                    //    return Ok(new
                    //    {
                    //        status = (int)ResponseType.SUCCESS,
                    //        msg = "Success"
                    //    });
                    //}

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

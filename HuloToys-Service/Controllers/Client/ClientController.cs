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
using Caching.Elasticsearch;
using HuloToys_Service.Models.Queue;
using HuloToys_Service.Utilities.constants;
using HuloToys_Service.Controllers.Order.Business;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly AccountClientESService accountClientESService;
        private readonly ClientESService clientESService;
        private readonly IdentiferService _identifierServiceRepository;
        public ClientController(IConfiguration _configuration) {
            configuration= _configuration;
            workQueueClient=new WorkQueueClient(configuration);
            accountClientESService = new AccountClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            clientESService = new ClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            _identifierServiceRepository = new IdentiferService();
        }
        [HttpPost("login")]
        public async Task<ActionResult> ClientLogin([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientLoginRequestModel>(objParr[0].ToString());
                    if (request == null 
                        || request.user_name == null || request.user_name.Trim() == ""
                        || request.password == null || request.password.Trim() == ""
                        || request.type<0)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Tài khoản / Mật khẩu không chính xác, vui lòng thử lại"
                        });
                    }
                    switch (request.type)
                    {
                        case (int)AccountLoginType.Password:
                            {
                                //-- By Username 
                                var account_client = accountClientESService.GetByUsernameAndPassword(request.user_name, request.password);
                                if (account_client != null && account_client.id > 0 && account_client.clientid > 0)
                                {
                                    var client = clientESService.GetById((long)account_client.clientid);
                                    if (client != null && client.id > 0)
                                    {

                                        return Ok(new
                                        {
                                            status = (int)ResponseType.SUCCESS,
                                            msg = "Success",
                                            data = new
                                            {
                                                account_client_id = account_client.id,
                                                user_name = account_client.username,
                                                name = client.clientname
                                            }
                                        });

                                    }

                                }
                                //-- By Email 
                                var client_exitst = clientESService.GetByEmail(request.user_name.Split("@")[0]);
                                if(client_exitst!=null && client_exitst.Count > 0)
                                {
                                    client_exitst = client_exitst.Where(x => x.email.ToLower().Trim() == request.user_name.ToLower().Trim()).ToList();
                                    foreach(var client in client_exitst)
                                    {
                                        var account_client_exists = accountClientESService.GetByClientIdAndPassword(client.id, request.password);
                                        if (account_client_exists != null && account_client_exists.id > 0)
                                        {
                                            return Ok(new
                                            {
                                                status = (int)ResponseType.SUCCESS,
                                                msg = "Success",
                                                data = new
                                                {
                                                    account_client_id = account_client_exists.id,
                                                    user_name = account_client_exists.username,
                                                    name = client.clientname
                                                }
                                            });
                                        }
                                    }
                                }
                                //-- By Phone 
                                client_exitst = clientESService.GetByPhone(request.user_name);
                                if (client_exitst != null && client_exitst.Count > 0)
                                {
                                    foreach (var client in client_exitst)
                                    {
                                        var account_client_exists = accountClientESService.GetByClientIdAndPassword(client.id, request.password);
                                        if (account_client_exists != null && account_client_exists.id > 0)
                                        {
                                            return Ok(new
                                            {
                                                status = (int)ResponseType.SUCCESS,
                                                msg = "Success",
                                                data = new
                                                {
                                                    account_client_id = account_client_exists.id,
                                                    user_name = account_client_exists.username,
                                                    name = client.clientname
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                            break;
                        case (int)AccountLoginType.Google:
                            {
                                var account_client = accountClientESService.GetByUsernameAndGoogleToken(request.user_name, request.token);
                                if (account_client != null && account_client.id > 0 && account_client.clientid > 0)
                                {
                                    var client = clientESService.GetById((long)account_client.clientid);
                                    if (client != null && client.id > 0)
                                    {

                                        return Ok(new
                                        {
                                            status = (int)ResponseType.SUCCESS,
                                            msg = "Success",
                                            data = new
                                            {
                                                account_client_id = account_client.id,
                                                user_name = account_client.username,
                                                name = client.clientname
                                            }
                                        });

                                    }

                                }
                            }
                            break;
                        default:
                            {

                            }break;
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

        [HttpPost("register")]
        public async Task<ActionResult> ClientRegister([FromBody] APIRequestGenericModel input)
        {
            try
            {
                

                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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
                    string username_generate = "u" + DateTime.Now.ToString("yyMMddHHmmss");
                    for (int i = 1; i < 999; i++)
                    {
                        var value = username_generate + i.ToString().PadLeft(3, '0');
                        var exists = accountClientESService.GetByUsername(value);
                        if (exists != null) { continue; }
                        else
                        {
                            username_generate = value; 
                            break;
                        }
                    }

                    AccountClientViewModel model = new AccountClientViewModel()
                    {
                        ClientId = -1,
                        ClientType = 0,
                        Email = request.email == null || request.email.Trim() == "" ? "" : request.email.Trim(),
                        Id = -1,
                        isReceiverInfoEmail = request.is_receive_email == true ? (byte)1 : (byte)0,
                        Name = request.user_name.Trim(),
                        ClientName = request.user_name.Trim(),
                        Password = request.password,
                        Phone = request.phone,
                        Status = 0,
                        UserName = username_generate,
                        GoogleToken = request.token,
                        ClientCode =await _identifierServiceRepository.buildClientNo(0)
                    };
                    var queue_model = new ClientConsumerQueueModel()
                    {
                        data_push = JsonConvert.SerializeObject(model),
                        type = QueueType.ADD_USER
                    };
                    bool result= workQueueClient.InsertQueueSimple(new QueueSettingViewModel()
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
                            msg = "Success",
                            data= username_generate
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

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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
                    var account_client = accountClientESService.GetByUsername(request.name);
                    if (account_client != null&& account_client.id >0&& account_client.clientid >0) {
                        var client = clientESService.GetById((long)account_client.clientid);
                        if (client != null && client.id > 0)
                        {
                            string forgot_password_token = "";
                            //Generate new Forgot password token:
                            AccountClientViewModel model = new AccountClientViewModel()
                            {
                                ClientId =client.id,
                                ClientType = 0,
                                Email = null,
                                Id = account_client.id,
                                isReceiverInfoEmail = null,
                                Name = null,
                                Password = null,
                                Phone = null,
                                Status = 0,
                                UserName = null,
                                ForgotPasswordToken = forgot_password_token
                            };
                            var queue_model = new ClientConsumerQueueModel()
                            {
                                data_push = JsonConvert.SerializeObject(model),
                                type = QueueType.UPDATE_USER
                            };
                            bool result = workQueueClient.InsertQueueSimple(new  QueueSettingViewModel()
                            {
                                host = configuration["Queue:Host"],
                                port = Convert.ToInt32(configuration["Queue:Port"]),
                                v_host = configuration["Queue:V_Host"],
                                username = configuration["Queue:Username"],
                                password = configuration["Queue:Password"],
                            }, JsonConvert.SerializeObject(queue_model), QueueName.queue_app_push);
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
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
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

                    var account_client = accountClientESService.GetById(request.id);
                    if (account_client != null && account_client.id > 0 && account_client.clientid > 0)
                    {
                        var client = clientESService.GetById((long)account_client.clientid);
                        if (client != null && client.id > 0)
                        {
                            string new_password = CommonHelper.MD5Hash(request.password);
                            //Generate new Forgot password token:


                            AccountClientViewModel model = new AccountClientViewModel()
                            {
                                ClientId = client.id,
                                ClientType = 0,
                                Email = null,
                                Id = account_client.id,
                                isReceiverInfoEmail = null,
                                Name = null,
                                Password = new_password,
                                Phone = null,
                                Status = 0,
                                UserName = null,
                                ForgotPasswordToken = null
                            };
                            var queue_model = new ClientConsumerQueueModel()
                            {
                                data_push = JsonConvert.SerializeObject(model),
                                type = QueueType.UPDATE_USER
                            };
                            bool result = workQueueClient.InsertQueueSimple(new  QueueSettingViewModel()
                            {
                                host = configuration["Queue:Host"],
                                port = Convert.ToInt32(configuration["Queue:Port"]),
                                v_host = configuration["Queue:V_Host"],
                                username = configuration["Queue:Username"],
                                password = configuration["Queue:Password"],
                            }, JsonConvert.SerializeObject(queue_model), QueueName.queue_app_push);
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

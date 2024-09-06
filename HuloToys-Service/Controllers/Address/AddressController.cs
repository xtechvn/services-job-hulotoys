﻿using Caching.Elasticsearch;
using Entities.Models;
using HuloToys_Service.Controllers.Address.Business;
using HuloToys_Service.Models;
using HuloToys_Service.Models.Address;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.Client;
using HuloToys_Service.Models.Queue;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Address
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly AccountClientESService accountClientESService;
        private readonly AddressClientESService addressClientESService;
        private readonly AddressClientService addressClientService;
        private readonly RedisConn redisService;
        private readonly WorkQueueClient work_queue;
        public AddressController(IConfiguration _configuration, RedisConn _redisService)
        {
            configuration = _configuration;
            redisService = _redisService;
            accountClientESService = new AccountClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            work_queue = new WorkQueueClient(_configuration);
            addressClientESService = new AddressClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            redisService.Connect();
            addressClientService = new AddressClientService(_configuration, redisService);

        }
        [HttpPost("insert-address")]

        public async Task<IActionResult> insertAddress([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<AddressViewModel>(objParr[0].ToString());
                    bool response_queue = false;
                    
                    if (request != null && request.AccountClientId>0)
                    {
                        var account_client = accountClientESService.GetById(request.AccountClientId);
                        request.ClientId =(long)account_client.clientid;
                        var j_param = new Dictionary<string, object>
                        {
                            {"data_push", JsonConvert.SerializeObject(request)}, // có thể là json
                            {"type",QueueType.ADD_ADDRESS}
                        };
                        var _data_push = JsonConvert.SerializeObject(j_param);

                        // Execute Push Queue

                        response_queue = work_queue.InsertQueueSimple(_data_push, QueueName.queue_app_push);
                        //-- Clear Cache:
                        var cache_name = CacheType.ADDRESS_CLIENT + request.AccountClientId;
                        redisService.clear(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                        if (response_queue)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Success"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "FAILED"
                            });
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
        [HttpPost("update-address")]
        public async Task<IActionResult> updateAddress([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<AddressViewModel>(objParr[0].ToString());
                    bool response_queue = false;
                    var work_queue = new WorkQueueClient(configuration);
                  
                    if (request != null && request.AccountClientId > 0)
                    {
                        var account_client = accountClientESService.GetById(request.AccountClientId);
                        request.ClientId = (long)account_client.clientid;
                        var j_param = new Dictionary<string, object>
                    {
                        {"data_push", JsonConvert.SerializeObject(request)}, // có thể là json
                        {"type",QueueType.UPDATE_ADDRESS}
                    };
                        var _data_push = JsonConvert.SerializeObject(j_param);

                        // Execute Push Queue

                        response_queue = work_queue.InsertQueueSimple(_data_push, QueueName.queue_app_push);
                        //-- Clear Cache:
                        var cache_name = CacheType.ADDRESS_CLIENT + request.AccountClientId;
                        redisService.clear(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                        cache_name = CacheType.ADDRESS_CLIENT_DETAIL + request.Id + request.AccountClientId;
                        redisService.clear(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));

                        if (response_queue)
                        {

                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Success"
                            });

                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "FAILED"
                            });

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
        [HttpPost("list")]
        public async Task<IActionResult> AddressByClient([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientAddressGeneralRequestModel>(objParr[0].ToString());
                    if (request == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.ADDRESS_CLIENT + request.account_client_id;
                    var j_data = await redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim() != "")
                    {
                        ClientAddressListResponseModel result = JsonConvert.DeserializeObject<ClientAddressListResponseModel>(j_data);
                        if (result != null && result.list != null && result.list.Count > 0)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    var model = addressClientService.AddressByClient(request);
                    if (model.list != null && model.list.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(model), Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = model
                    });
                }


            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid,
            });
        }
        [HttpPost("default")]
        public async Task<IActionResult> AddressDefaultByClient([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientAddressGeneralRequestModel>(objParr[0].ToString());
                    if (request == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.ADDRESS_CLIENT + request.account_client_id;
                    var j_data = await redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim() != "")
                    {
                        ClientAddressListResponseModel result = JsonConvert.DeserializeObject<ClientAddressListResponseModel>(j_data);
                        if (result != null && result.list != null && result.list.Count > 0)
                        {
                            var selected_address = result.list.FirstOrDefault(x => x.isactive == true);
                            if (selected_address == null) selected_address = result.list.FirstOrDefault();
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = selected_address,
                            });
                        }
                    }
                    var model = addressClientService.AddressByClient(request);
                    if (model.list != null && model.list.Count > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(model), Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    }
                    var selected = model.list.FirstOrDefault(x => x.isactive == true);
                    if (selected == null) selected = model.list.FirstOrDefault();
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = selected
                    });
                }


            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid,
            });
        }
        [HttpPost("detail")]
        public async Task<IActionResult> AddressDetail([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ClientAddressDetailRequestModel>(objParr[0].ToString());
                    if (request == null || request.account_client_id <= 0 || request.id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.ADDRESS_CLIENT_DETAIL + request.id + request.account_client_id;
                    var j_data = await redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim() != "")
                    {
                        AddressClientESModel result = JsonConvert.DeserializeObject<AddressClientESModel>(j_data);
                        if (result != null && result.id > 0)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    var account_client = accountClientESService.GetById(request.account_client_id);
                    var detail = addressClientESService.GetById(request.id, (long)account_client.clientid);
                    if (detail != null && detail.id > 0)
                    {
                        redisService.Set(cache_name, JsonConvert.SerializeObject(detail), Convert.ToInt32(configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = detail
                    });
                }


            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid,
            });
        }
    }
}

﻿using HuloToys_Front_End.Models.Products;
using HuloToys_Service.Controllers.Product.Bussiness;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.Client;
using HuloToys_Service.Models.Label;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HuloToys_Service.Controllers.Label
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabelController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILabelRepository _labelRepository;
        private readonly RedisConn _redisService;

        public LabelController(IConfiguration configuration, RedisConn redisService, ILabelRepository labelRepository)
        {
            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            _labelRepository = labelRepository;
        }
        [HttpPost("list")]
        public async Task<IActionResult> Listing([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {

                    var request = JsonConvert.DeserializeObject<LabelListingRequestModel>(objParr[0].ToString());
                    if (request == null || request.page_index<1 || request.page_size<0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    string cache_name =CacheType.LABEL;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_common"]));
                    List<LabelListingModel> result = null;
                    if (j_data != null && j_data.Trim() != "")
                    {
                        result = JsonConvert.DeserializeObject<List<LabelListingModel>>(j_data);
                    }
                    if((request.page_size * request.page_index)>200)
                    {
                        result = await _labelRepository.Listing(0, null, request.page_index, request.page_size);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = ResponseMessages.Success,
                            data = result.Select(x => new {
                                x.Id,
                                x.LabelName,
                                x.Icon,
                                x.LabelCode
                            })
                        });
                    }
                    else if(result!=null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = ResponseMessages.Success,
                            data = result.Skip((request.page_index-1)*request.page_size).Take(request.page_size).Select(x => new {
                                x.Id,
                                x.LabelName,
                                x.Icon,
                                x.LabelCode
                            })
                        });
                    }
                    result = await _labelRepository.Listing(0,null,1,200);
                    if (result != null && result.Count>0)
                    {
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(result), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = result.Skip((request.page_index - 1) * request.page_size).Take(request.page_size).Select(x => new {
                            x.Id,
                            x.LabelName,
                            x.Icon,
                            x.LabelCode
                        })
                    });
                }

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString();
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid,
            });
        }
    }
}

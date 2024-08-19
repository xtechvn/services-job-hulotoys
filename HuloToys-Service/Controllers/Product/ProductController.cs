﻿using Entities.ViewModels.Products;
using HuloToys_Front_End.Models.Products;
using HuloToys_Service.ElasticSearch.NewEs;
using HuloToys_Service.RedisWorker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.APIRequest;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Models.Product;

namespace WEB.CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;
        private readonly GroupProductESService groupProductESService;

        public ProductController(IConfiguration configuration, RedisConn redisService)
        {
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            groupProductESService = new GroupProductESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);

            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
        }
       
        [HttpPost("get-list")]
        public async Task<IActionResult> ProductListing([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductListRequestModel>(objParr[0].ToString());
                    if (request == null )
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.PRODUCT_LISTING + (request.keyword ?? "") + request.group_id + request.page_index + request.page_size;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim()!="")
                    {
                        ProductListResponseModel result = JsonConvert.DeserializeObject<ProductListResponseModel>(j_data);
                        if(result!=null && result.items != null)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    if (request.page_size <= 0) request.page_size = 10;
                    if (request.page_index < 1) request.page_index = 1;
                    var data = await _productDetailMongoAccess.Listing(request.keyword, request.group_id, request.page_index, request.page_size);
                    if (data != null && data.items != null && data.items.Count > 0)
                    {
                        _redisService.Set(cache_name,JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = data
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
        [HttpPost("get-list-sub")]
        public async Task<IActionResult> ProductSubListing([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductDetailRequestModel>(objParr[0].ToString());
                    if (request == null||request.id == null|| request.id.Trim()=="")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.PRODUCT_SUB_LISTING +  request.id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim() != "")
                    {
                        ProductListResponseModel result = JsonConvert.DeserializeObject<ProductListResponseModel>(j_data);
                        if (result != null && result.items != null)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    var data = await _productDetailMongoAccess.SubListing(new List<string>() { request.id });
                    if (data != null && data.items != null && data.items.Count>0)
                    {
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = data
                    });

                }
                   
            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Failed",
            });
        }
        [HttpPost("detail")]
        public async Task<IActionResult> ProductDetail([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductDetailRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var cache_name = CacheType.PRODUCT_SUB_LISTING + request.id;
                    var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    if (j_data != null && j_data.Trim() != "")
                    {
                        ProductMongoDbModel result = JsonConvert.DeserializeObject<ProductMongoDbModel>(j_data);
                        if (result != null )
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    var data = await _productDetailMongoAccess.GetByID(request.id);
                    if (data != null)
                    {
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = data
                    });

                }
               
            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Failed",
            });
        }
        [HttpPost("group-product")]
        public async Task<IActionResult> GroupProduct([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductListRequestModel>(objParr[0].ToString());
                    if (request == null || request.group_id <=0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = groupProductESService.GetListGroupProductByParentId(request.group_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = data
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
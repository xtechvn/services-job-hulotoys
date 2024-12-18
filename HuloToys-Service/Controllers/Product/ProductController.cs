﻿using Caching.Elasticsearch;
using Entities.ViewModels.Products;
using HuloToys_Front_End.Models.Products;
using HuloToys_Service.Controllers.Product.Bussiness;
using HuloToys_Service.ElasticSearch;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Models.Raiting;
using HuloToys_Service.MongoDb;
using HuloToys_Service.RedisWorker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;
using Utilities.Contants;

namespace WEB.CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        private readonly ProductSpecificationMongoAccess _productSpecificationMongoAccess;
        private readonly CartMongodbService _cartMongodbService;
        private readonly RaitingESService _raitingESService;
        private readonly OrderDetailESService orderDetailESService;
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;
        private readonly GroupProductESService groupProductESService;
        private readonly ProductRaitingService productRaitingService;
        private readonly ProductDetailService productDetailService;

        public ProductController(IConfiguration configuration, RedisConn redisService)
        {
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            _productSpecificationMongoAccess = new ProductSpecificationMongoAccess(configuration);
            _cartMongodbService = new CartMongodbService(configuration);
            productRaitingService = new ProductRaitingService(configuration);
            productDetailService = new ProductDetailService(configuration);
            orderDetailESService = new OrderDetailESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            groupProductESService = new GroupProductESService(configuration["DataBaseConfig:Elastic:Host"], configuration);
            _raitingESService = new RaitingESService(configuration["DataBaseConfig:Elastic:Host"], configuration);

            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
        }

        [HttpPost("get-list")]
        public async Task<IActionResult> ProductListing([FromBody] APIRequestGenericModel input)
        {
            try
            {
                //input.token = "F081O1oSKR4nJktCB3d5ekEyMysRMQY0LBBoCGN6TgYGUTYtKygpBxF9Xn85";
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductListRequestModel>(objParr[0].ToString());
                    if (request == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    if (request.keyword ==null) request.keyword  = "";
                    if (request.page_size <= 0) request.page_size = 10;
                    if (request.page_index < 1) request.page_index = 1;
                    //var cache_name = CacheType.PRODUCT_LISTING + (request.keyword ?? "") + request.group_id+ request.page_index+ request.page_size;
                    //var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //if (j_data != null && j_data.Trim() != "")
                    //{
                    //    ProductListResponseModel result = JsonConvert.DeserializeObject<ProductListResponseModel>(j_data);
                    //    if (result != null && result.items != null && result.items.Count >0)
                    //    {
                    //        return Ok(new
                    //        {
                    //            status = (int)ResponseType.SUCCESS,
                    //            msg = ResponseMessages.Success,
                    //            data = result
                    //        });
                    //    }
                    //}

                    var data = await productDetailService.ProductListing(request);
                   
                    //if (data != null  && data.items.Count > 0)
                    //{
                    //    _redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //}
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
                    //var cache_name = CacheType.PRODUCT_DETAIL + request.id;
                    //var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //if (j_data != null && j_data.Trim() != "")
                    //{
                    //    ProductDetailResponseModel result = JsonConvert.DeserializeObject<ProductDetailResponseModel>(j_data);
                    //    if (result != null)
                    //    {
                    //        return Ok(new
                    //        {
                    //            status = (int)ResponseType.SUCCESS,
                    //            msg = ResponseMessages.Success,
                    //            data = result
                    //        });
                    //    }
                    //}
                    var data = await _productDetailMongoAccess.GetFullProductById(request.id);
                    //if (data != null)
                    //{
                    //    _redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //}
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
                    if (request == null || request.group_id <= 0)
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
        [HttpPost("search")]
        public async Task<IActionResult> ProductSearch([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductGlobalSearchRequestModel>(objParr[0].ToString());
                    if (request == null|| request.keyword == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    //var cache_name = CacheType.PRODUCT_SEARCH + (request.keyword ?? "");
                    //var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //if (j_data != null && j_data.Trim() != "")
                    //{
                    //    ProductListResponseModel result = JsonConvert.DeserializeObject<ProductListResponseModel>(j_data);
                    //    if (result != null && result.items != null && result.items.Count > 0)
                    //    {
                    //        return Ok(new
                    //        {
                    //            status = (int)ResponseType.SUCCESS,
                    //            msg = ResponseMessages.Success,
                    //            data = result
                    //        });
                    //    }
                    //}

                    var data = await _productDetailMongoAccess.Search(request.keyword);

                    //if (data != null && data.items.Count > 0)
                    //{
                    //    _redisService.Set(cache_name, JsonConvert.SerializeObject(data), Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                    //}
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
        
        [HttpPost("raiting-count")]
        public async Task<IActionResult> ProductRaitingCount([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductRaitingRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    ProductRaitingResponseModel result = _raitingESService.CountCommentByProductId(request.id);
                    List<string> product_ids = new List<string>()
                    {
                        request.id
                    };
                    var product=await _productDetailMongoAccess.SubListing(request.id);
                    if(product!=null && product.Count > 0)
                    {
                        product_ids.AddRange(product.Select(x => x._id));
                    }
                    result.total_sold = orderDetailESService.CountByProductId(product_ids);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = result
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
        [HttpPost("raiting")]
        public async Task<IActionResult> ProductRaiting([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductRaitingRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    if (request.page_index < 1) request.page_index = 1;
                    if (request.page_size < 1) request.page_size = 5;
                    var data= await productRaitingService.GetListByFilter(request);
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
        [HttpPost("global-search-filter")]
        public async Task<IActionResult> ProductGlobalSearchFilter([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductGlobalSearchRequestModel>(objParr[0].ToString());
                    if (request == null || request.keyword == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = await _productDetailMongoAccess.GlobalSearch("", 0, "", "", 1, 500);
                    List<ProductSpecificationDetailMongoDbModel> brands = new List<ProductSpecificationDetailMongoDbModel>();
                    List<GroupProductESModel> groups = new List<GroupProductESModel>();
                    ProductListResponseModel items = new ProductListResponseModel();
                    if (data!=null && data.items!=null && data.items.Count > 0)
                    {
                        var value = string.Join(",", data.items.Select(x => x.group_product_id));
                        var ids = value.Split(",").Where(x=>x!=null && x.Trim()!="").Select(x => Convert.ToInt64(x)).ToList();
                        groups =  groupProductESService.GetGroupProductByIDs(ids);
                        brands = data.items.Where(x=>x.specification!=null && x.specification.Count>0).SelectMany(x => x.specification).Where(x=>x.attribute_id==1).Distinct().ToList();
                        brands = brands.Where(x => x.value != null &&x.value != "null" && x.value.Trim() != "").DistinctBy(x => x.value).ToList();
                        string brand_split = string.Join(",", brands.Select(x => x.value));
                        brands = brand_split.Split(",").Distinct().Select(x => new ProductSpecificationDetailMongoDbModel()
                        {
                            attribute_id = 1,
                            value = x,
                            value_type = 1,
                            type_ids = "1",
                            _id = ""
                        }).ToList();
                        items = new ProductListResponseModel()
                        {
                            items = data.items.Take(12).ToList(),
                            count = data.count
                        };
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data= items,
                        brands = brands,
                        groups= groups
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
        [HttpPost("global-search")]
        public async Task<IActionResult> ProductGlobalSearch([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductGlobalSearchRequestModel>(objParr[0].ToString());
                    if (request == null || request.keyword == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    if (request.page_index == null || request.page_index <= 0) request.page_index = 1;
                    if (request.page_size == null || request.page_size <= 0) request.page_index = 12;
                    var data = await _productDetailMongoAccess.GlobalSearch(request.keyword, request.stars, request.group_product_id, request.brands, (int)request.page_index, (int)request.page_size);
                  
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
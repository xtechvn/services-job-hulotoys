using Entities.ViewModels.Products;
using HuloToys_Front_End.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProductController(IConfiguration configuration)
        {
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            _configuration=configuration;

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
                    if (request.page_size <= 0) request.page_size = 10;
                    if (request.page_index < 1) request.page_index = 1;
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = await _productDetailMongoAccess.Listing(request.keyword, request.group_id, request.page_index, request.page_size)
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
                   
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = await _productDetailMongoAccess.SubListing(new List<string>() { request.id })
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
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = await _productDetailMongoAccess.GetByID(request.id)
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
    }
  
}
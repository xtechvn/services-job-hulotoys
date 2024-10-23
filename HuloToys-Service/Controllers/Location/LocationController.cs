using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using HuloToys_Service.Utilities.Lib;
using Utilities;
using Utilities.Contants;
using Caching.Elasticsearch;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Models.Location;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly LocationESService locationESService;

        public LocationController(IConfiguration _configuration, RedisConn redisService) {
            configuration= _configuration;
            locationESService = new LocationESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }
        [HttpPost("province")]
        public async Task<ActionResult> Province([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<LocationRequestModel>(objParr[0].ToString());
                    if (request == null)
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var province_es = locationESService.GetAllProvinces();
                    if (province_es != null && province_es.Count > 0)
                    {
                        List<Province> data = province_es.Select(x => new Models.Location.Province()
                        {
                            CreatedDate = x.createddate,
                            Id = x.id,
                            Name = x.name,
                            NameNonUnicode = x.namenonunicode,
                            ProvinceId = x.provinceid,
                            Status = x.status,
                            Type = x.type
                        }).ToList();
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = data
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
        [HttpPost("district")]
        public async Task<ActionResult> District([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<LocationRequestModel>(objParr[0].ToString());
                    if (request == null || request.id==null || request.id.Trim()=="")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var district_es = locationESService.GetAllDistrictByProvinces(request.id);
                    if (district_es != null && district_es.Count > 0)
                    {
                        List<District> data = district_es.Select(x => new Models.Location.District()
                        {
                            CreatedDate = x.createddate,
                            Id = x.id,
                            Name = x.name,
                            NameNonUnicode = x.namenonunicode,
                            ProvinceId = x.provinceid,
                            Status = x.status,
                            Type = x.type,
                            DistrictId = x.districtid,
                            Location = x.location

                        }).ToList();
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = data
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
        [HttpPost("ward")]
        public async Task<ActionResult> Ward([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<LocationRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var ward_es = locationESService.GetAllWardsByDistrictId(request.id);
                    if (ward_es != null && ward_es.Count > 0)
                    {
                        List<Ward> data = ward_es.Select(x => new Models.Location.Ward()
                        {
                            CreatedDate = x.createddate,
                            Id = x.id,
                            Name = x.name,
                            NameNonUnicode = x.namenonunicode,
                            WardId = x.wardid,
                            Status = x.status,
                            Type = x.type,
                            DistrictId = x.districtid,
                            Location = x.location
                        }).ToList();
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Success",
                            data = data
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
    }
}

using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.RedisWorker;

using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using Utilities.Contants;
//cuonglv
namespace HuloToys_Service.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly RedisConn redisService;
        public TestController(IConfiguration _configuration, RedisConn _redisService)
        {
            configuration = _configuration;
            redisService = _redisService;
        }
        /// <summary>
        /// Test login 
        /// </summary>
        /// <returns></returns>
        [HttpGet("verify-authent.json")]
        [Authorize] // Bật login lấy token      
        public async Task<IActionResult> verifyAuthent()
        {
            try
            {
                return Ok(new { msg = "You are authenticated" });
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return null;
            }
        }

        /// <summary>
        /// Test ElasticSearch
        /// </summary>
        /// <returns></returns>
        [HttpGet("test-elastic.json")]
        [Authorize] // Bật login lấy token      
        public async Task<IActionResult> testElastic()
        {
            try
            {
                var index_es = configuration["DataBaseConfig:Elastic:index_product"];
                var es_host = configuration["DataBaseConfig:Elastic:Host"];

                IESRepository<object> _ESRepository = new ESRepository<object>(es_host,configuration);
                var result_product = _ESRepository.FindById("product","đồ chơi","product_id");
                return Ok(new { data = result_product });
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new { error = 1, msg = ex.ToString() });
            }
        }

        /// <summary>
        /// Test RABBITMQ
        /// </summary>
        /// <returns></returns>
        [HttpPost("test-push-queue.json")]
        [Authorize] // Bật login lấy token      
        public async Task<IActionResult> testRabbitMQ()
        {
            try
            {
                bool response_queue = false;
                var j_param = new Dictionary<string, object>
                {
                    {"data_push","test message queue"}, // có thể là json
                    {"type",QueueType.ADD_ADDRESS}
                };
                var _data_push = JsonConvert.SerializeObject(j_param);

                // Execute Push Queue
                var work_queue = new WorkQueueClient(configuration);
                var queue_setting = new QueueSettingViewModel
                {
                    host = configuration["Queue:Host"],
                    v_host = configuration["Queue:V_Host"],
                    port = Convert.ToInt32(configuration["Queue:Port"]),
                    username = configuration["Queue:Username"],
                    password = configuration["Queue:Password"]
                };
                response_queue = work_queue.InsertQueueSimple(queue_setting, _data_push, QueueName.queue_app_push);
                if (response_queue)
                {
                    return Ok(new { msg = "done" });
                }
                else
                {
                    return Ok(new { msg = "fail" });
                }                
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new { msg = ex.ToString() });
            }
        }
    }
}

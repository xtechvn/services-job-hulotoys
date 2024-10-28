using Caching.Elasticsearch;
using Entities.Models;
using HuloToys_Service.Models.Client;
using Microsoft.Extensions.Caching.Memory;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Shipping.Business
{
    public class NinjaVanService
    {
        private readonly IConfiguration _configuration;
        private IMemoryCache _MemoryCache;
        private string token="";
        private RestClient ninja_van_client;

        public NinjaVanService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            var options = new RestClientOptions(_configuration["NinjaVan:APIs:Domain"]);
            ninja_van_client = new RestClient(options);
            _MemoryCache = memoryCache;
            var rs= GetBearerToken().Result;
        }
        public int CaclucateShippingFee(AddressClientESModel address, int weight_in_grams)
        {
            int shipping_fee = 0;
            try
            {

            }
            catch
            {

            }
            return shipping_fee;
        }
        public async Task<bool> GetBearerToken()
        {
            var result = false;
            try
            {
                var request = new RestRequest(_configuration["NinjaVan:APIs:OAuth"], Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var model = new
                {
                    client_id = _configuration["NinjaVan:client_id"],
                    client_secret = _configuration["NinjaVan:client_secret"],
                    grant_type = _configuration["NinjaVan:grant_type"],
                };
                var body = JsonConvert.SerializeObject(model);
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = await ninja_van_client.ExecuteAsync(request);
                var jsonData = JObject.Parse(response.Content);
                var response_token = jsonData["access_token"].ToString();

                if (response_token != null && response_token.Trim()!="")
                {
                    token= response_token.Trim();
                }
                result = true;
            }
            catch
            {

            }
            return result;
        }
    }
}

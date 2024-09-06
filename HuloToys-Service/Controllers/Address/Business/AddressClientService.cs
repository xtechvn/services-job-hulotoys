using Caching.Elasticsearch;
using Entities.Models;
using HuloToys_Service.Models.Client;
using HuloToys_Service.Models.Location;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.RedisWorker;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Address.Business
{
    public class AddressClientService
    {
        private readonly IConfiguration configuration;
        private readonly AccountClientESService accountClientESService;
        private readonly AddressClientESService addressClientESService;
        private readonly RedisConn redisService;
        public AddressClientService(IConfiguration _configuration, RedisConn _redisService)
        {
            configuration = _configuration;
            redisService = _redisService;
            accountClientESService = new AccountClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            addressClientESService = new AddressClientESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }
        public ClientAddressListResponseModel AddressByClient(ClientAddressGeneralRequestModel request)
        {
            ClientAddressListResponseModel model = new ClientAddressListResponseModel()
            {
                list = new List<AddressClientFEModel>()
            };
            try
            {
                var account_client = accountClientESService.GetById(request.account_client_id);
                var client_id = (long)account_client.clientid;
               
                var list = addressClientESService.GetByClientID(client_id);
                var provinces = redisService.Get(CacheType.PROVINCE, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var district = redisService.Get(CacheType.DISTRICT, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var ward = redisService.Get(CacheType.WARD, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var data_provinces = JsonConvert.DeserializeObject<List<Province>>(provinces);
                var data_district =  JsonConvert.DeserializeObject<List<District>>(district);
                var data_ward =JsonConvert.DeserializeObject<List<Ward>>(ward);

                if (list!=null && list.Count > 0)
                {
                    foreach (var item in list) {
                        AddressClientFEModel submit_item = JsonConvert.DeserializeObject<AddressClientFEModel>(JsonConvert.SerializeObject(item));
                        if (submit_item != null) {
                            submit_item.province_detail = data_provinces.FirstOrDefault(x => x.ProvinceId == submit_item.provinceid);
                            submit_item.district_detail = data_district.FirstOrDefault(x => x.DistrictId == submit_item.districtid);
                            submit_item.ward_detail = data_ward.FirstOrDefault(x => x.WardId == submit_item.wardid);
                            model.list.Add(submit_item);
                        }
                    }
                }
               
            }
            catch
            {

            }
            return model;
        }
        public AddressClientFEModel DefaultAddress(ClientAddressGeneralRequestModel request)
        {
            AddressClientFEModel model = new AddressClientFEModel();
            try
            {
                var account_client = accountClientESService.GetById(request.account_client_id);
                var client_id = (long)account_client.clientid;

                var list = addressClientESService.GetByClientID(client_id);
                var provinces = redisService.Get(CacheType.PROVINCE, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var district = redisService.Get(CacheType.DISTRICT, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var ward = redisService.Get(CacheType.WARD, Convert.ToInt32(configuration["Redis:Database:db_common"]));
                var data_provinces = JsonConvert.DeserializeObject<List<Province>>(provinces);
                var data_district = JsonConvert.DeserializeObject<List<District>>(district);
                var data_ward = JsonConvert.DeserializeObject<List<Ward>>(ward);

                if (list != null && list.Count > 0)
                {
                    var selected = list.FirstOrDefault(x => x.isactive == true);
                    if (selected == null) selected = list.FirstOrDefault();
                    model = JsonConvert.DeserializeObject<AddressClientFEModel>(JsonConvert.SerializeObject(selected));
                    model.province_detail = data_provinces.FirstOrDefault(x => x.ProvinceId == selected.provinceid);
                    model.district_detail = data_district.FirstOrDefault(x => x.DistrictId == selected.districtid);
                    model.ward_detail = data_ward.FirstOrDefault(x => x.WardId == selected.wardid);
                }

            }
            catch
            {

            }
            return model;
        }
    }
}

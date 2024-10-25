using APP_CHECKOUT.Elasticsearch;
using APP_CHECKOUT.Helpers;
using APP_CHECKOUT.Models.Client;
using APP_CHECKOUT.Models.NhanhVN;
using APP_CHECKOUT.Models.Orders;
using Caching.Elasticsearch;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_CHECKOUT.Repositories
{
    public class NhanhVnService
    {
        private readonly IConfiguration _configuration;
        private readonly LocationESService locationESService;

        public NhanhVnService(IConfiguration configuration)
        {

            _configuration = configuration;
            locationESService = new LocationESService(configuration["Elastic:Host"], configuration);

        }
        public async Task PostToNhanhVN(Order order_summit, OrderDetailMongoDbModel order, ClientESModel client, AddressClientESModel address_client)
        {
            try
            {

                var options = new RestClientOptions(_configuration["NhanhVN:API:Domain"]);
                var http_client = new RestClient(options);
                var request = new RestRequest(_configuration["NhanhVN:API:Domain"] + _configuration["NhanhVN:API:AddOrder"], Method.Post);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("version", "2.0");
                request.AddParameter("appId", _configuration["NhanhVN:AppId"]);
                request.AddParameter("businessId", _configuration["NhanhVN:BussinessID"]);
                request.AddParameter("accessToken", _configuration["NhanhVN:AppId"]);
                var city = await GetLocationByType(0);
                string city_name = "";
                string district_name = "";
                string ward_name = "";
                var province = locationESService.GetProvincesByProvinceId(order.provinceid);
                var district = locationESService.GetDistrictByDistrictId(order.districtid);
                var ward = locationESService.GetWardsByWardId(order.wardid);
                var city_map = city.FirstOrDefault(x => CommonHelpers.RemoveUnicode(x.name.Trim().ToLower()).Contains(CommonHelpers.RemoveUnicode(province.Name.Trim().ToLower())));
                if(city_map!=null && city_map.id > 0)
                {
                    city_name=city_map.name;
                    var districts=await GetLocationByType(1, city_map.id);
                    if (districts != null && districts.Count > 0)
                    {
                        var district_map = districts.FirstOrDefault(x => CommonHelpers.RemoveUnicode(x.name.Trim().ToLower()).Contains(CommonHelpers.RemoveUnicode(district.Name.Trim().ToLower())));
                        if(district_map != null && district_map.name != null)
                        {
                            district_name = district_map.name;
                            var wards = await GetLocationByType(2, district_map.id);
                            if (wards != null && wards.Count > 0)
                            {
                                var ward_map = districts.FirstOrDefault(x => CommonHelpers.RemoveUnicode(x.name.Trim().ToLower()).Contains(CommonHelpers.RemoveUnicode(district.Name.Trim().ToLower())));
                                ward_name = ward_map == null || ward_map.name == null ? "" : ward_map.name;
                            }
                        }
                      
                    }
                }
                NhanhVNOrderAddRequestModel model = new NhanhVNOrderAddRequestModel()
                {
                    id = order_summit.OrderId.ToString(),
                    depotId = null,
                    type = "Shipping",
                    customerName = address_client.receivername,
                    customerMobile = address_client.phone,
                    customerEmail = client.email,
                    customerAddress = order_summit.Address,
                    customerCityName = city_name,
                    customerDistrictName= district_name,
                    customerWardLocationName=ward_name,
                    moneyDiscount=order_summit.Discount,
                    moneyTransfer= null,
                    moneyTransferAccountId=null,
                    moneyDeposit=null,
                    moneyDepositAccountId = null,
                    paymentMethod= "COD",
                    paymentCode = null,
                    paymentGateway=null,
                    carrierId=null,
                    carrierServiceId=null,
                    customerShipFee=0,
                    deliveryDate=DateTime.Now.AddDays(2).ToString("yyyy-mm-dd"),
                    status="New",
                    description = null,
                    privateDescription=null,
                    trafficSource= null,
                    couponCode=order.voucher_id.ToString(),
                    allowTest=1,
                    saleId=null,
                    autoSend=0,
                    sendCarrierType=2,
                    carrierAccountId=null,
                    carrierShopId=null,
                    carrierServiceCode= null,
                    utmCampaign= null,
                    utmMedium= null,
                    utmSource= null,
                    isPartDelivery=null,
                    usedPoint=null,
                    affiliate = null,
                    productList=new List<NhanhVNOrderProductRequestModel>()
                };
                foreach(var c in order.carts)
                {
                  
                    model.productList.Add(new NhanhVNOrderProductRequestModel()
                    {
                        id=c.product._id,
                        code=c.product.sku,
                        description=CommonHelpers.RemoveSpecialCharacters(c.product.description),
                        idNhanh=null,
                        imei=null,
                        name=c.product.name,
                        importPrice=Convert.ToInt32(c.product.price),
                        price= Convert.ToInt32(c.product.amount),
                        quantity=c.quanity,
                        weight = Convert.ToInt32(c.product.weight==null?0: (float)c.product.weight),
                        gifts=null,
                        type="Product"
                    });
                }

                request.AddParameter("data", JsonConvert.SerializeObject(model));

                RestResponse response = await http_client.ExecuteAsync(request);
                Console.WriteLine(response.Content);


            }
            catch
            {

            }


        }
        public async Task<List<NhanhVNLocationResponseLocation>> GetLocationByType(int type, int parent_id=0)
        {
            try
            {
                string type_string = "CITY";
                switch (type){
                    case 0:
                        {

                        }break;
                    case 1:
                        {
                            type_string = "DISTRICT";
                        }
                        break;
                    case 2:
                        {
                            type_string = "WARD";

                        }
                        break;
                    default:
                        {
                            return new List<NhanhVNLocationResponseLocation>();
                        }

                }
                var options = new RestClientOptions(_configuration["NhanhVN:API:Domain"]);
                var http_client = new RestClient(options);
                var request = new RestRequest(_configuration["NhanhVN:API:Domain"]+ _configuration["NhanhVN:API:Location"], Method.Post);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("version", "2.0");
                request.AddParameter("appId", _configuration["NhanhVN:AppId"]);
                request.AddParameter("businessId", _configuration["NhanhVN:BussinessID"]);
                request.AddParameter("accessToken", _configuration["NhanhVN:AppId"]);
                request.AddParameter("data", "{\"type\":\""+ type_string + "\",\"parentId\":"+ parent_id + "}");
              
                RestResponse response = await http_client.ExecuteAsync(request);
                var jsonData = JObject.Parse(response.Content);
                var status = int.Parse(jsonData["code"].ToString());

                if (status == 0)
                {
                    var data= JsonConvert.DeserializeObject<List<NhanhVNLocationResponseLocation>>(jsonData["data"].ToString());
                    return data;
                }

            }
            catch
            {

            }
            return new List<NhanhVNLocationResponseLocation>();

        }
    }
}

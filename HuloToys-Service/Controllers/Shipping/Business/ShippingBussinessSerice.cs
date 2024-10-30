using HuloToys_Service.Models.NinjaVan;
using HuloToys_Service.MongoDb;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.constants.NinjaVan;
using HuloToys_Service.Utilities.constants.Shipping;

namespace HuloToys_Service.Controllers.Shipping.Business
{
    public class ShippingBussinessSerice
    {
        private readonly IConfiguration configuration;
        private readonly NinjaVanService ninjaVanService;
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        private readonly CartMongodbService _cartMongodbService;

        public ShippingBussinessSerice(IConfiguration _configuration)
        {
            configuration = _configuration;
            ninjaVanService = new NinjaVanService(configuration);
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            _cartMongodbService = new CartMongodbService(configuration);

        }
        public async Task<ShippingFeeResponseModel> GetShippingFeeResponse(ShippingFeeRequestModel request)
        {
            try
            {
                var response = new ShippingFeeResponseModel()
                {
                    from_province_id = request.from_province_id<=0 ? NinjaVanShippingFee.WareHouse_Province_id.First(): request.from_province_id,
                    to_province_id = request.to_province_id,
                    type = request.shipping_type,
                    cart_id=request.cart_id,
                    product_id=request.product_id,
                    quanity=request.quanity,
                    shipping_fee= 0

                };
                //-- Delivery type
                switch (request.shipping_type)
                {
                    case (int)ShippingType.FAST_DELIVERY:
                        {
                            //-- switch delivery carrier
                            switch (request.carrier_id)
                            {
                                case (int)ShippingCarrier.NINJAVAN:
                                    {
                                        var cart_detail = await _cartMongodbService.FindById(request.cart_id);

                                        if (cart_detail != null && cart_detail._id != null)
                                        {
                                            var shipping_fee = ninjaVanService.CaclucateShippingFee(request.to_province_id, Convert.ToInt32(cart_detail.product.weight));
                                            if (shipping_fee > 0)
                                            {
                                                response.shipping_fee = shipping_fee * (request.quanity <= 1 ? 1 : request.quanity);
                                            }
                                            else
                                            {
                                                response.shipping_fee = -1;
                                            }
                                        }
                                    }
                                    break;
                            }

                        }
                        break;
                    case (int)ShippingType.SLOW_DELIVERY:
                        {


                        }
                        break;
                    case (int)ShippingType.INSTANT_DELIVERY:
                        {


                        }
                        break;
                    case (int)ShippingType.RECEIVER_AT_WAREHOUSE:
                        {
                            response.shipping_fee = 0;
                        }
                        break;
                    case (int)ShippingType.COD:
                        {


                        }
                        break;

                }
                return response;
            }
            catch
            {

            }
            return null;
        }
    }
}

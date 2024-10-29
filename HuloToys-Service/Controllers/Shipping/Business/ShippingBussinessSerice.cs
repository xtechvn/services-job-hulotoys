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
                    total_shipping_fee = 0,
                    type = request.shipping_type,
                    detail = new List<ShippingFeeResponseShippingFee>()

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
                                        var list_fee = new List<ShippingFeeResponseShippingFee>();
                                        foreach (var cart in request.carts)
                                        {
                                            var cart_detail = await _cartMongodbService.FindById(cart.id);

                                            if (cart_detail != null && cart_detail._id != null)
                                            {
                                                var shipping_fee = ninjaVanService.CaclucateShippingFee(request.to_province_id, Convert.ToInt32(cart_detail.product.weight));
                                                if (shipping_fee > 0)
                                                {
                                                    list_fee.Add(new ShippingFeeResponseShippingFee()
                                                    {
                                                        cart_id = cart.id,
                                                        product_id = cart.product_id,
                                                        quanity = cart.quanity,
                                                        shipping_fee = shipping_fee *(cart.quanity<=1?1:cart.quanity)
                                                    });
                                                    response.total_shipping_fee += (shipping_fee * (cart.quanity <= 1 ? 1 : cart.quanity));
                                                }
                                            }

                                        }
                                        response.detail = list_fee;
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
                            var list_fee = new List<ShippingFeeResponseShippingFee>();
                            foreach (var cart in request.carts)
                            {
                                list_fee.Add(new ShippingFeeResponseShippingFee()
                                {
                                    cart_id = cart.id,
                                    product_id = cart.product_id,
                                    quanity = cart.quanity,
                                    shipping_fee = 0
                                });
                            }
                            response.detail = list_fee;
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

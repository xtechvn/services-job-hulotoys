using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System.Threading.Tasks;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
    public interface IOrderHotelRentsService
    {
        int createOrder(HotelRentOrderSummit order_info, OrderEntities order_queue);
        Task<HotelRentOrderSummit> HotelRentSummitFromMessage(OrderEntities message);
        Task<OrderWithOldPaymentType> UpdatePaymentTypeWithOrderID(OrderEntities order_info);

    }
}

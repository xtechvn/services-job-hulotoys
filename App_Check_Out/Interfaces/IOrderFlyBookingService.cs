using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System.Threading.Tasks;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
    public interface IOrderFlyBookingService
   {
        int createOrder(FlyBookingOrderSummit order_info, OrderEntities order_queue);
        Task<FlyBookingOrderSummit> flyBookingOrderFromMessage(OrderEntities message);
        Task<OrderWithOldPaymentType> UpdatePaymentTypeWithOrderID(OrderEntities order_info);
    }
}

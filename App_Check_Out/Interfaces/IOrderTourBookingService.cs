using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
    public interface IOrderTourBookingService
    {
        Task<TourSummit> TourFromMessage(OrderEntities message);
        int createOrder(TourSummit order_info, OrderEntities order_queue);
        Task<OrderInfo> GetOrderInfo(long order_id);
    }
}

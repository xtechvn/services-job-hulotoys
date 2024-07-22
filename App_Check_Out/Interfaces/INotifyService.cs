using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
   public interface INotifyService
    {
        bool pushNotiCreateOrder(int order_id);
        bool pushNotiUpdateOrderPaymentType(OrderWithOldPaymentType orderItems);
        Task<int> SendNotifyCMS(long order_id);
    }
}

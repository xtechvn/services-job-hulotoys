using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
    public interface IOrderVinWonderService
    {
        int createOrder(VinWonderTicketSummit order_info, OrderEntities order_queue);
        Task<VinWonderTicketSummit> VinWonderTicketFromMessage(OrderEntities message);

    }
}

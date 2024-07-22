using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
  public interface ILogActivityService
    {
        bool logCreateOrder(OrderEntities order_info);
        bool logUpdateOrderPaymentType(OrderEntities order_info);
    }
}

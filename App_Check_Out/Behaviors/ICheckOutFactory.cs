using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Behaviors
{
  public  interface ICheckOutFactory
    {
        void DoSomeRealWork(OrderEntities order);
    }
}

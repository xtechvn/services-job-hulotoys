using App_Push_Consummer.Model.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface IOrderBusiness
    {
        Task<Int32> UpdateOrder(OrderModel data);
    }
}

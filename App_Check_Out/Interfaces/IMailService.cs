using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Interfaces
{
    public interface IMailService
    {
        bool sendMailOrderB2C(int order_id);
        bool sendMailOrderB2B(int order_id);
        bool sendMailVinWonder(int order_id);
        bool sendMailHotelBooking(int orderid);
    }
}

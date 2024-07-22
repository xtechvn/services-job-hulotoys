using ADAVIGO_FRONTEND_B2C.Models.Tour.TourBooking;
using System;
using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class OrderEntities
    {
        public long order_id { get; set; }
        public string payment_type { get; set; }
        public string client_id { get; set; }
        public string account_client_id { get; set; }
        public string bank_code { get; set; }
        public string booking_cart_id { get; set; }
        public string session_id { get; set; }
        public string booking_id { get; set; }
        public string order_no { get; set; }
        public int service_type { get; set; }
        public int event_status { get; set; }
        public long? contract_id { get; set; }

    }


}

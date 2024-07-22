using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
   public class HotelPaymentModel
    {
        public string hotelID { get; set; }
        public string hotelName { get; set; }
        public DateTime arrivalDate { get; set; }
        public DateTime departureDate { get; set; }
        public int numberOfRoom { get; set; }
        public int numberOfAdult { get; set; }
        public int numberOfChild { get; set; }
        public int numberOfInfant { get; set; }
        public decimal totalMoney { get; set; }
        public string bookingID { get; set; }
        public string orderID { get; set; }
    }
}

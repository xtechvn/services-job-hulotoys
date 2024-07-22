using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public class HotelGuest
    {
        public int Id { get; set; }
        public int HotelBookingRoomsID { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public int HotelBookingId { get; set; }
    }
}

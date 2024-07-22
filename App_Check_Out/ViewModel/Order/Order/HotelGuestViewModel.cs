using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class HotelGuestViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public long HotelBookingId { get; set; }
        public long HotelBookingRoomsID { get; set; }
        public string Note { get; set; } = "";
        public short? Type { get; set; }

    }
}

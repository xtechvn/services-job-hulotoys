using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class HotelRentOrderSummit
    {
        public int client_type { get; set; }
        public OrderViewModel obj_order { get; set; }
        public ContactClientViewModel obj_contact_client { get; set; }

        public List<HotelRentOrderSummitDetail> obj_hotel_rent { get; set; }
    }
    public class HotelRentOrderSummitDetail
    {
        public HotelBooking booking { get; set; }
        public List<HotelBookingRoomsSummit> rooms { get; set; }
    }

    public class HotelBookingRoomsSummit
    {
        public HotelBookingRooms detail { get; set; }
        public HotelBookingRoomsOptional rooms_optional { get; set; }

        public List<HotelGuestViewModel> guests { get; set; }

        public List<HotelBookingRoomsSummitRate> rates { get; set; }

    }
    public class HotelBookingRoomsSummitRate
    {


        public HotelBookingRoomRates rates { get; set; }
        public HotelBookingRoomRatesOptional rates_optional { get; set; }

    }
}

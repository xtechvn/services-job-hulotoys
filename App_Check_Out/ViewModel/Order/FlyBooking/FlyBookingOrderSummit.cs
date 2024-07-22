using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class FlyBookingOrderSummit
    {
        public int client_type { get; set; }
        public OrderViewModel obj_order { get; set; }
        public ContactClientViewModel obj_contact_client { get; set; }
        public List<PassengerViewModel> passengers { get; set; }

        public List<FlyBookingDetailViewModel> obj_fly_booking { get; set; }
        public VoucherViewModel voucher { get; set; }
        public FlyBookingOrderSummitAdditional additional { get; set; }
    }

    public class FlyBookingOrderSummitAdditional
    {
        public string note_go { get; set; }
        public string note_back { get; set; }
    }
}

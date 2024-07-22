using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder
{
    public class VinWonderTicketSummit
    {
        public OrderViewModel order { get; set; }
        public ContactClientViewModel obj_contact_client { get; set; }
        public List<VinWonderBooking> bookings { get; set; }
        public VoucherViewModel voucher { get; set; }

    }
}

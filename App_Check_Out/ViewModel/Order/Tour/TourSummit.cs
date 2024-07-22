using APP.CHECKOUT_SERVICE.ViewModel.Tour;
using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder
{
    public class TourSummit
    {
        public OrderViewModel order { get; set; }
        public ContactClientViewModel obj_contact_client { get; set; }
        public TourViewModel booking { get; set; }
        public List<TourPackages> packages { get; set; }
        public VoucherViewModel voucher { get; set; }

    }
}

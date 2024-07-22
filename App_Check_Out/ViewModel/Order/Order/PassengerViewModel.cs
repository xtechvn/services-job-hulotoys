using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class PassengerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MembershipCard { get; set; }
        public string PersonType { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Gender { get; set; }
        public long OrderId { get; set; }
        public List<BaggageViewModel> baggages { get; set; }
        public string Note { get; set; }
        public string GroupBookingId { get; set; }

    }
}

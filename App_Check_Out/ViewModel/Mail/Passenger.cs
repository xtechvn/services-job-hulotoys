using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public partial class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MembershipCard { get; set; }
        public string PersonType { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Gender { get; set; }
        public long OrderId { get; set; }
    }
}

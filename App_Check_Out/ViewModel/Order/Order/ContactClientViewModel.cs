using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
   public class ContactClientViewModel
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public long ClientId { get; set; }
        public int? Id { get; set; }
        public long OrderId { get; set; }
    }
   
}

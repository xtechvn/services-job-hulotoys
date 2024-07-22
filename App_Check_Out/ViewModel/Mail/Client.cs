using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public class Client
    {
        public long ClientId { get; set; }
        public int? ClientMapId { get; set; }
        public int? SaleMapId { get; set; }
        public int? ClientType { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public string Avartar { get; set; }
        public DateTime JoinDate { get; set; }
        public bool? IsReceiverInfoEmail { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string TaxNo { get; set; }
        public int? ParentId { get; set; }
    }
}


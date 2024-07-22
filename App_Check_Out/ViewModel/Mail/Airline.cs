using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public partial class Airlines
    {
        public string Code { get; set; }
        public string NameVi { get; set; }
        public string NameEn { get; set; }
        public string AirLineGroup { get; set; }
        public string Logo { get; set; }
        public int? SupplierId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

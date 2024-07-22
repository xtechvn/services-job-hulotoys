using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class VoucherViewModel
    {
        public int status { get; set; }
        public string expire_date { get; set; }
        public string voucher_name { get; set; }
        public double discount { get; set; }
        public double total_order_amount_before { get; set; }
        public double total_order_amount_after { get; set; }
        public double percent_decrease { get; set; }
        public int voucher_id { get; set; }


    }
}

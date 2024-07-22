using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Log
{
    public class SystemLog
    {

        public int SourceID { get; set; } // log từ nguồn nào, quy định trong SystemLogSourceID
        public string Type { get; set; } // nội dung: booking, order,....
        public string KeyID { get; set; } // Key: mã đơn, mã khách hàng, mã booking,....
        public string Log { get; set; } // nội dung log
    }
 
}

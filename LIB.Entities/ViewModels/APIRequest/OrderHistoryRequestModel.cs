using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.APIRequest
{
    public class OrderHistoryRequestModel
    {
        public long client_id { get; set; }
        public string order_no { get; set; }
    }
}

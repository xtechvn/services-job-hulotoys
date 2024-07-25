using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.APIRequest
{
    public class CartDeleteRequestModel
    {
        public string id { get; set; }
        public long client_id { get; set; }
    }
}

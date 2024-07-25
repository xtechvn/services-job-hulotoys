using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ElasticSearch
{
    public class OrderESModel
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; } 
        public string OrderStatus { get; set; }
        public DateTime? createtime { get; set; }
    }
}

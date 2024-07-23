using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Model.Address
{
    public class AddressModel
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdateTime { get; set; }

    }
}

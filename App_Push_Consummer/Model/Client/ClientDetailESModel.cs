using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Model.Client
{
    public class ClientDetailESModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string ClientName { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
    }
}

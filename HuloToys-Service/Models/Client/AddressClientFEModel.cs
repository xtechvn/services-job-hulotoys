﻿using HuloToys_Service.Models.Models;

namespace Entities.Models
{
    public partial class AddressClientFEModel : AddressClientESModel
    {
      public Province province_detail { get; set; }
      public District district_detail { get; set; }
      public Ward ward_detail { get; set; }

    }


}

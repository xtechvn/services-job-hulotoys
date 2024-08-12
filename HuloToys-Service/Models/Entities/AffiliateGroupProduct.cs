using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class AffiliateGroupProduct
{
    public int Id { get; set; }

    public int GroupProductId { get; set; }

    public int AffType { get; set; }
}

using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models;

public partial class RatingESModel
{
    public int id { get; set; }

    public int? orderid { get; set; }

    public string productid { get; set; }

    public decimal? star { get; set; }

    public string comment { get; set; }

    public string imglink { get; set; }

    public string videolink { get; set; }

    public int? userid { get; set; }

    public DateTime? createddate { get; set; }

    public DateTime? updateddate { get; set; }

    public int? updatedby { get; set; }
}

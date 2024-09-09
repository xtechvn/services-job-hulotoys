using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Location;

public class Province
{
    public int Id { get; set; }

    public string ProvinceId { get; set; }

    public string Name { get; set; }

    public string NameNonUnicode { get; set; }

    public string Type { get; set; }

    public short? Status { get; set; }
}

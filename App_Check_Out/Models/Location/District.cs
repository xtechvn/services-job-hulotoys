﻿using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Location;

public class District
{
    public int Id { get; set; }

    public string DistrictId { get; set; }

    public string Name { get; set; }

    public string NameNonUnicode { get; set; }

    public string Type { get; set; }

    public string Location { get; set; }

    public string ProvinceId { get; set; }

    public short? Status { get; set; }
}

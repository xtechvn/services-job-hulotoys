using System;
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
    public DateTime? CreatedDate { get; set; }

}
public class DistrictESModel
{
    public int id { get; set; }

    public string districtid { get; set; }

    public string name { get; set; }

    public string namenonunicode { get; set; }

    public string type { get; set; }

    public string location { get; set; }

    public string provinceid { get; set; }

    public short? status { get; set; }
    public DateTime? createddate { get; set; }

}

using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Location;

public class Ward
{
    public int Id { get; set; }

    public string WardId { get; set; }

    public string Name { get; set; }

    public string NameNonUnicode { get; set; }

    public string Type { get; set; }

    public string Location { get; set; }

    public string DistrictId { get; set; }

    public short? Status { get; set; }
    public DateTime? CreatedDate { get; set; }

}
public class WardESModel
{
    public int id { get; set; }

    public string wardid { get; set; }

    public string name { get; set; }

    public string namenonunicode { get; set; }

    public string type { get; set; }

    public string location { get; set; }

    public string districtid { get; set; }

    public short? status { get; set; }
    public DateTime? createddate { get; set; }

}
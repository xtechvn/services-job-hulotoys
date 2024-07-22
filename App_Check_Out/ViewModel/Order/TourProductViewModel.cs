using System;
using System.Collections.Generic;

namespace Entities.ViewModels.Tour
{


    public class TourProductSPModel : TourProduct
    {
        public IEnumerable<TourProductScheduleModel> TourSchedule { get; set; }
        public IEnumerable<string> OtherImages { get; set; }
        public IEnumerable<int> EndPoints { get; set; }
        public string SupplierName { get; set; }
    }

    public class TourProductScheduleModel
    {
        public int day_num { get; set; }
        public string day_title { get; set; }
        public string day_description { get; set; }
    }

  
    public partial class TourProduct
    {
        public long Id { get; set; }
        public string TourName { get; set; }
        public string ServiceCode { get; set; }
        public int? TourType { get; set; }
        public int? StartPoint { get; set; }
        public int? OrganizingType { get; set; }
        public string DateDeparture { get; set; }
        public double Price { get; set; }
        public int? SupplierId { get; set; }
        public int? Status { get; set; }
        public int? Days { get; set; }
        public double? OldPrice { get; set; }
        public int? Star { get; set; }
        public string Avatar { get; set; }
        public bool? IsDisplayWeb { get; set; }
        public string Image { get; set; }
        public string Schedule { get; set; }
        public string AdditionInfo { get; set; }
        public bool? IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsSelfDesigned { get; set; }
        public string Transportation { get; set; }
        public string Description { get; set; }
        public string Include { get; set; }
        public string Exclude { get; set; }
        public string Refund { get; set; }
        public string Surcharge { get; set; }
        public string Note { get; set; }
    }
}

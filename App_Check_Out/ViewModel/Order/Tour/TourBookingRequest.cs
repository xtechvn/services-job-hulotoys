using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADAVIGO_FRONTEND_B2C.Models.Tour.TourBooking
{
    public class TourBookingRequest
    {
        public TourBookingRequestContact contact { get; set; }
        public long tour_product_id { get; set; }
        public DateTime start_date { get; set; }
        public TourBookingRequestGuest guest { get; set; }
        public string voucher_name { get; set; }
        public long account_client_id { get; set; }
        public string note { get; set; }
        public TourProduct tour_product { get; set; }
        public TourProgramPackages packages { get; set; }
    }

    public class TourBookingRequestContact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }

    }
    public class TourBookingRequestGuest
    {
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }


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
    public partial class TourProgramPackages
    {
        public long Id { get; set; }
        public long? TourProductId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsDaily { get; set; }
        public double? AdultPrice { get; set; }
        public double? ChildPrice { get; set; }
        public int? ClientType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

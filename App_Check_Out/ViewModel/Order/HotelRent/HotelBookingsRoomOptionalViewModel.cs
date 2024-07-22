using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order.HotelRent
{
   public class HotelBookingsRoomOptionalViewModel : HotelBookingRoomsOptionalModel
    {
        public decimal TotalAmountPay { get; set; }
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? NumberOfAdult { get; set; }
        public int? NumberOfChild { get; set; }
        public int? NumberOfInfant { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public double SaleTotalAmount { get; set; }
        public double UnitPrice { get; set; }
        public double SaleNumberOfRoom { get; set; }
        public double Amount { get; set; }
    }
    public partial class HotelBookingRoomsOptionalModel
    {
        public long Id { get; set; }
        public long HotelBookingId { get; set; }
        public long HotelBookingRoomId { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public double TotalAmount { get; set; }
        public short? NumberOfRooms { get; set; }
        public int? SupplierId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string PackageName { get; set; }
        public bool? IsRoomFund { get; set; }
    }
    public class HotelBookingRoomRatesOptionalViewModel : HotelBookingRoomRatesOptionalModel
    {
        public string RatePlanCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? SalePrice { get; set; }
        public int? Nights { get; set; }
        public long HotelBookingRoomId { get; set; }
        public long SupplierId { get; set; }
        public string RatePlanId { get; set; }
        public DateTime StayDate { get; set; }
        public double? SaleTotalAmount { get; set; }
    }
    public partial class HotelBookingRoomRatesOptionalModel
    {
        public long Id { get; set; }
        public long HotelBookingRoomOptionalId { get; set; }
        public long HotelBookingRoomRatesId { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public double TotalAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public double? OperatorPrice { get; set; }
    }
}

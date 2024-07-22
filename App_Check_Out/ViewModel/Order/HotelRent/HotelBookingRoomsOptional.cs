using System;
using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public partial class HotelBookingRoomsOptional
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
}

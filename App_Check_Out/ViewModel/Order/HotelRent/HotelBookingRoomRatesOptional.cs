using System;
using System.Collections.Generic;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public partial class HotelBookingRoomRatesOptional
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

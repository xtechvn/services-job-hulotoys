﻿using System;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public partial class FlyBookingDetail
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PriceId { get; set; }
        public int? Status { get; set; }
        public int? BookingId { get; set; }
        public string BookingCode { get; set; }
        public string Flight { get; set; }
        public double Amount { get; set; }
        public double? Profit { get; set; }
        public double? Difference { get; set; }
        public string Currency { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? Leg { get; set; }
        public string Session { get; set; }
        public string Airline { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string GroupClass { get; set; }
        public int? AdultNumber { get; set; }
        public int? ChildNumber { get; set; }
        public int? InfantNumber { get; set; }
        public double? FareAdt { get; set; }
        public double? FareChd { get; set; }
        public double? FareInf { get; set; }
        public double? TaxAdt { get; set; }
        public double? TaxChd { get; set; }
        public double? TaxInf { get; set; }
        public double? FeeAdt { get; set; }
        public double? FeeChd { get; set; }
        public double? FeeInf { get; set; }
        public double? ServiceFeeAdt { get; set; }
        public double? ServiceFeeChd { get; set; }
        public double? ServiceFeeInf { get; set; }
        public double? AmountAdt { get; set; }
        public double? AmountChd { get; set; }
        public double? AmountInf { get; set; }
        public double? TotalNetPrice { get; set; }
        public double? TotalDiscount { get; set; }
        public double? TotalCommission { get; set; }
        public double? TotalBaggageFee { get; set; }
        public long? SalerId { get; set; }
        public string ServiceCode { get; set; }
        public string GroupBookingId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

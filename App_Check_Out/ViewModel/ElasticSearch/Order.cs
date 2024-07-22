using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class Order
    {
      
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public byte? ServiceType { get; set; }
        public DateTime? CreateTime { get; set; }
        public double? Amount { get; set; }
        public int? PaymentStatus { get; set; }
        public long? ClientId { get; set; }
        public long? ContactClientId { get; set; }
        public byte? OrderStatus { get; set; }
        public long? ContractId { get; set; }
        public string SmsContent { get; set; }
        public int? PaymentType { get; set; }
        public string BankCode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentNo { get; set; }
        public string ColorCode { get; set; }
        public double? Discount { get; set; }
        public double? Profit { get; set; }
        public DateTime? ExpriryDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProductService { get; set; }
        public string Note { get; set; }
        public int? UtmSource { get; set; }
        public DateTime? UpdateLast { get; set; }
        public long? SalerId { get; set; }
        public string SalerGroupId { get; set; }
        public long? UserUpdateId { get; set; }
        public short? SystemType { get; set; }
        public long? AccountClientId { get; set; }

        public class VoucherViewModel
        {
        }
    }
}

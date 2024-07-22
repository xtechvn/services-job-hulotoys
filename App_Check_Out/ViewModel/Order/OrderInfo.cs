using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class OrderInfo
    {
        public long OrderId { get; set; }
        public long AccountClientId { get; set; }
        public string OrderNo { get; set; }
        public byte? ServiceType { get; set; }
        public DateTime? CreateTime { get; set; }
        public double? Amount { get; set; }
        public long? ClientId { get; set; }
        public int? ContactClientId { get; set; }
        public byte? OrderStatus { get; set; }
        public long? ContractId { get; set; }
        public string SmsContent { get; set; }
        public int? PaymentType { get; set; }
        public string BankCode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime ExpriryDate { get; set; }
        public string PaymentNo { get; set; }
        public string VoucherCode { get; set; }
        public int? VoucherId { get; set; }
        public double? Discount { get; set; }
        public long? SalerId { get; set; }
    }
}

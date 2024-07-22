using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
  public  class OrderViewModel
    {
        public long Id { get; set; }
        public long Exists_id { get; set; }
        public string OrderNo { get; set; }
        public byte? ServiceType { get; set; }
        public DateTime? CreateTime { get; set; }
        public double? Amount { get; set; }
        public long? ClientId { get; set; }
        public int? ContactClientId { get; set; }
        public byte? OrderStatus { get; set; }
        public long? ContractId { get; set; }
        public int? PaymentType { get; set; }
        public byte? PaymentStatus { get; set; }
        public string BankCode { get; set; }
        public DateTime? PaymentDate { get; set; }
        

        public string PaymentNo { get; set; }
        public double? Discount { get; set; }
        public double? Profit { get; set; }
        public DateTime? ExpriryDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProductService { get; set; }
        public long? AccountClientId { get; set; }
        public short? SystemType { get; set; }
        public long? SalerId { get; set; }
        public string SalerGroupId { get; set; }
        public long? UserUpdateId { get; set; }

        public string SmsContent { get; set; }
        public string Note { get; set; }
        public string UtmSource { get; set; }
        public DateTime? UpdateLast { get; set; }
        public long? CreatedBy { get; set; }
        public string Description { get; set; }
        public short? BranchCode { get; set; }
        public string BookingInfo { get; set; }
        public string Label { get; set; }
        public bool? IsFinishPayment { get; set; }
        public int? PercentDecrease { get; set; }
        public int? VoucherId { get; set; }
        public int SupplierId { get; set; }
        public double? Price { get; set; }
        public string UtmMedium { get; set; }

    }
    public class OrderWithOldPaymentType: OrderInfo
    {
        public int? old_payment_type { get; set; }
    }
}

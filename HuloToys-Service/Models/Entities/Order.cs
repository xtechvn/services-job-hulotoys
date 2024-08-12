using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class Order
{
    public long OrderId { get; set; }

    public string OrderNo { get; set; } = null!;

    public byte? ServiceType { get; set; }

    public DateTime? CreateTime { get; set; }

    public double? Amount { get; set; }

    public int? PaymentStatus { get; set; }

    public long? ClientId { get; set; }

    public long? ContactClientId { get; set; }

    public byte? OrderStatus { get; set; }

    public string? SmsContent { get; set; }

    public int? PaymentType { get; set; }

    public string? BankCode { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentNo { get; set; }

    public string? ColorCode { get; set; }

    public double? Discount { get; set; }

    public double? Profit { get; set; }

    public DateTime? ExpriryDate { get; set; }

    /// <summary>
    /// ngay bat dau khoi tao dich vu
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Ngay ket thuc dich vu
    /// </summary>
    public DateTime? EndDate { get; set; }

    public string? ProductService { get; set; }

    /// <summary>
    /// Chính là label so với wiframe
    /// </summary>
    public string? Note { get; set; }

    public string? UtmSource { get; set; }

    public DateTime? UpdateLast { get; set; }

    public long? SalerId { get; set; }

    public string? SalerGroupId { get; set; }

    public long? UserUpdateId { get; set; }

    public short? SystemType { get; set; }

    public long? AccountClientId { get; set; }

    public long? CreatedBy { get; set; }

    public string? Description { get; set; }

    public short? BranchCode { get; set; }

    public string? BookingInfo { get; set; }

    public string? Label { get; set; }

    public short? IsFinishPayment { get; set; }

    public int? PercentDecrease { get; set; }

    public int? VoucherId { get; set; }

    public double? Price { get; set; }

    public int? SupplierId { get; set; }

    public int? DepartmentId { get; set; }

    public string? OperatorId { get; set; }

    public int? UserVerify { get; set; }

    public DateTime? VerifyDate { get; set; }

    public int? DebtStatus { get; set; }

    public string? DebtNote { get; set; }

    public double? Commission { get; set; }

    public string? UtmMedium { get; set; }

    public double? Refund { get; set; }

    public virtual ContactClient? ContactClient { get; set; }
}

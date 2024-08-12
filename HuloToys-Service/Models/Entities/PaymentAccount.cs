using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class PaymentAccount
{
    public int Id { get; set; }

    /// <summary>
    /// Số tài khoản
    /// </summary>
    public string? AccountNumb { get; set; }

    /// <summary>
    /// Tên chủ tài khoản
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Tên ngân hàng
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Chi nhánh
    /// </summary>
    public string? Branch { get; set; }

    public long ClientId { get; set; }
}

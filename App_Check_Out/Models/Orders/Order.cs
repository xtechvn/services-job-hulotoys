﻿using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public long ClientId { get; set; }

    public string OrderNo { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdateLast { get; set; }

    public double? Price { get; set; }

    public double? Profit { get; set; }

    public double? Discount { get; set; }

    public double? Amount { get; set; }

    public int Status { get; set; }

    public short PaymentType { get; set; }

    public int PaymentStatus { get; set; }

    public string UtmSource { get; set; }

    public string UtmMedium { get; set; }

    /// <summary>
    /// Chính là label so với wiframe
    /// </summary>
    public string Note { get; set; }

    public int? VoucherId { get; set; }

    public int? IsDelete { get; set; }

    public int? UserId { get; set; }
    public string UserGroupIds { get; set; }

}

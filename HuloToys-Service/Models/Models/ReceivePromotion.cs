﻿using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Models;

public partial class ReceivePromotion
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}

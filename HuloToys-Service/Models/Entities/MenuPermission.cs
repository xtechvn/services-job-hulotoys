﻿using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class MenuPermission
{
    public int Id { get; set; }

    public int MenuId { get; set; }

    public int PermissionId { get; set; }
}

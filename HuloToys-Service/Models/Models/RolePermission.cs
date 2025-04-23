﻿using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int MenuId { get; set; }

    public int PermissionId { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

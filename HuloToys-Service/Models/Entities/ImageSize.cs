using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class ImageSize
{
    public int Id { get; set; }

    public string PositionName { get; set; } = null!;

    public int Width { get; set; }

    public int Height { get; set; }
}

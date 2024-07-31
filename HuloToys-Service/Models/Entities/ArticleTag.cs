using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class ArticleTag
{
    public long Id { get; set; }

    public long? TagId { get; set; }

    public long? ArticleId { get; set; }

    public virtual Article? Article { get; set; }

    public virtual Tag? Tag { get; set; }
}

﻿using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Entities;

public partial class ArticleRelated
{
    public long Id { get; set; }

    public long? ArticleId { get; set; }

    public long? ArticleRelatedId { get; set; }

    public virtual Article? Article { get; set; }
}

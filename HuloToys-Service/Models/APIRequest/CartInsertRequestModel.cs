﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.APIRequest
{
    public class CartInsertRequestModel
    {
        public long client_id { get; set; }
        public int product_id { get; set; }
    }
}
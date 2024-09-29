using App_Push_Consummer.Model.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface IProductRaitingService
    {
        Task<int> InsertRaiting(ProductRaitingPushQueueModel model);
    }
}

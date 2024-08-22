using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface ICommentsBusiness
    {
        Task<Int32> saveComments(CommentsModel data);
        Task<Int32> saveReceiverInfoEmail(CommentsModel data);
    }
}

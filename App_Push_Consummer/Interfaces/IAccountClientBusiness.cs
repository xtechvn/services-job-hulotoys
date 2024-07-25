using HuloToys_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface IAccountClientBusiness
    {
        Task<Int32> saveAccountClient(AccountClientModel data);
        Task<Int32> updateAccountClient(AccountClientModel data);
    }
}

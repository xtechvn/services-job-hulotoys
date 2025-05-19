using App_Push_Consummer.Model.Client;
using HuloToys_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Interfaces
{
    public interface IClientBusiness
    {
        Task<Int32> UpdateClient(ClientDetailESModel data);
    }
}

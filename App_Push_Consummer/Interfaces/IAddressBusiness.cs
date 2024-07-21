using App_Push_Consummer.Model.Address;

namespace App_Push_Consummer.Interfaces
{
    public interface IAddressBusiness
    {
        Task<Int32> saveAddress(AddressModel data);
    }
}

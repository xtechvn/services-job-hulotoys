using HuloToys_Service.Controllers.CarRegistration.Model;

namespace HuloToys_Service.IRepositories
{

    public interface IGoogleFormsService
    {
        Task<bool> SubmitToGoogleFormAsync(RegistrationRecord record);
    }
}

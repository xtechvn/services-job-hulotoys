using HuloToys_Service.Controllers.CarRegistration.Model;

namespace HuloToys_Service.IRepositories
{
    public interface IValidationService
    {
        ValidationResult ValidateCarRegistration(CarRegistrationRequest request);
        TimeRestrictionResult CheckTimeRestriction(string phoneNumber);
    }
}

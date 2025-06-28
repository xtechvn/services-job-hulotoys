using HuloToys_Service.Controllers.CarRegistration.Model;

namespace HuloToys_Service.IRepositories
{
    public interface IGoogleSheetsService
    {
        Task<int> GetDailyQueueCountAsync();
        Task<bool> SaveRegistrationAsync(RegistrationRecord record);
        Task<DateTime?> GetLastSubmissionTimeAsync(string phoneNumber);
        Task UpdateLastSubmissionTimeAsync(string phoneNumber, DateTime submissionTime);
    }
}

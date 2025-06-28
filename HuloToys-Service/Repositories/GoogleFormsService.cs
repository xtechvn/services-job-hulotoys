using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using System.Net.Http;

namespace HuloToys_Service.Repositories
{
    public class GoogleFormsService : IGoogleFormsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleFormsService> _logger;
        private const string GOOGLE_FORM_URL = "https://docs.google.com/forms/d/e/YOUR_FORM_ID/formResponse";

        public GoogleFormsService(IHttpClientFactory httpClientFactory, ILogger<GoogleFormsService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(); ;
            _logger = logger;
        }

        public async Task<bool> SubmitToGoogleFormAsync(RegistrationRecord record)
        {
            try
            {
                // Prepare form data for Google Forms submission
                var formData = new Dictionary<string, string>
                {
                    {"entry.123456789", record.PhoneNumber}, // Replace with actual entry IDs
                    {"entry.987654321", record.PlateNumber},
                    {"entry.555666777", record.QueueNumber.ToString()},
                    {"entry.111222333", record.RegistrationTime.ToString("yyyy-MM-dd HH:mm:ss")}
                };

                var content = new FormUrlEncodedContent(formData);

                // Submit to Google Form
                var response = await _httpClient.PostAsync(GOOGLE_FORM_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully submitted to Google Form: {record.PhoneNumber}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Google Form submission failed with status: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting to Google Form");
                return false;
            }
        }
    }
}

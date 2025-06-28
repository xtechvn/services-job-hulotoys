using Amazon.Runtime.Internal.Util;
using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using Microsoft.Extensions.Caching.Memory;

namespace HuloToys_Service.Repositories
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<GoogleSheetsService> _logger;

        public GoogleSheetsService(IMemoryCache cache, ILogger<GoogleSheetsService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<int> GetDailyQueueCountAsync()
        {
            try
            {
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var cacheKey = $"daily_count_{today}";

                if (_cache.TryGetValue(cacheKey, out int count))
                {
                    return count;
                }

                // In real implementation, query Google Sheets API
                // For demo, simulate random count
                await Task.Delay(100); // Simulate API call
                count = new Random().Next(0, 50);

                _cache.Set(cacheKey, count, TimeSpan.FromDays(1));
                _logger.LogInformation($"Retrieved daily queue count: {count}");

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily queue count");
                throw;
            }
        }

        public async Task<bool> SaveRegistrationAsync(RegistrationRecord record)
        {
            try
            {
                // In real implementation, save to Google Sheets
                await Task.Delay(200); // Simulate API call

                // Update daily count cache
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var cacheKey = $"daily_count_{today}";
                var currentCount = await GetDailyQueueCountAsync();
                _cache.Set(cacheKey, currentCount + 1, TimeSpan.FromDays(1));

                _logger.LogInformation($"Saved registration: {record.PhoneNumber} - {record.PlateNumber} - Queue: {record.QueueNumber}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving registration to Google Sheets");
                return false;
            }
        }

        public async Task<DateTime?> GetLastSubmissionTimeAsync(string phoneNumber)
        {
            try
            {
                var cacheKey = $"last_submission_{phoneNumber}";

                if (_cache.TryGetValue(cacheKey, out DateTime lastTime))
                {
                    return lastTime;
                }

                // In real implementation, query Google Sheets for last submission
                await Task.Delay(50); // Simulate API call
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting last submission time for {phoneNumber}");
                return null;
            }
        }

        public async Task UpdateLastSubmissionTimeAsync(string phoneNumber, DateTime submissionTime)
        {
            try
            {
                var cacheKey = $"last_submission_{phoneNumber}";
                _cache.Set(cacheKey, submissionTime, TimeSpan.FromMinutes(15));

                // In real implementation, update Google Sheets
                await Task.Delay(50); // Simulate API call

                _logger.LogInformation($"Updated last submission time for {phoneNumber}: {submissionTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating last submission time for {phoneNumber}");
                throw;
            }
        }
    }
}

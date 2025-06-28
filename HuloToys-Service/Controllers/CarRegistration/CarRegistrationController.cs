using HuloToys_Service.Controllers.CarRegistration.Model;
using HuloToys_Service.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HuloToys_Service.Controllers.CarRegistration
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarRegistrationController : Controller
    {
        private readonly IValidationService _validationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleFormsService _googleFormsService;
        private readonly ILogger<CarRegistrationController> _logger;
        public CarRegistrationController(
         IValidationService validationService,
         IGoogleSheetsService googleSheetsService,
         IGoogleFormsService googleFormsService,
         ILogger<CarRegistrationController> logger)
        {
            _validationService = validationService;
            _googleSheetsService = googleSheetsService;
            _googleFormsService = googleFormsService;
            _logger = logger;
        }
        [HttpPost("register")]
        [EnableRateLimiting("PhoneNumberPolicy")]
        public async Task<ActionResult<CarRegistrationResponse>> RegisterCar([FromBody] CarRegistrationRequest request)
        {
            try
            {
                _logger.LogInformation($"Car registration request received: {request.PhoneNumber} - {request.PlateNumber}");

                // Step 1: Validate input data
                var validationResult = _validationService.ValidateCarRegistration(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = string.Join(", ", validationResult.Errors)
                    });
                }

                // Step 2: Check time restriction (15 minutes rule)
                var timeRestriction = _validationService.CheckTimeRestriction(request.PhoneNumber);
                if (!timeRestriction.CanSubmit)
                {
                    return BadRequest(new CarRegistrationResponse
                    {
                        Success = false,
                        Message = $"Vui lòng đợi {timeRestriction.RemainingMinutes} phút trước khi gửi lại",
                        RemainingTimeMinutes = timeRestriction.RemainingMinutes
                    });
                }

                // Step 3: Get current daily queue count
                var dailyCount = await _googleSheetsService.GetDailyQueueCountAsync();
                var queueNumber = dailyCount + 1;

                // Step 4: Create registration record
                var registrationRecord = new RegistrationRecord
                {
                    PhoneNumber = request.PhoneNumber,
                    PlateNumber = request.PlateNumber.ToUpper(),
                    QueueNumber = queueNumber,
                    RegistrationTime = DateTime.Now
                };

                // Step 5: Submit to Google Form
                var formSubmissionSuccess = await _googleFormsService.SubmitToGoogleFormAsync(registrationRecord);
                if (!formSubmissionSuccess)
                {
                    _logger.LogWarning("Google Form submission failed, but continuing...");
                }

                // Step 6: Save to Google Sheets
                var sheetsSuccess = await _googleSheetsService.SaveRegistrationAsync(registrationRecord);
                if (!sheetsSuccess)
                {
                    return StatusCode(500, new CarRegistrationResponse
                    {
                        Success = false,
                        Message = "Lỗi hệ thống, vui lòng thử lại sau"
                    });
                }

                // Step 7: Update last submission time
                await _googleSheetsService.UpdateLastSubmissionTimeAsync(request.PhoneNumber, DateTime.Now);

                // Return success response
                return Ok(new CarRegistrationResponse
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    QueueNumber = queueNumber,
                    RegistrationTime = registrationRecord.RegistrationTime,
                    PlateNumber = registrationRecord.PlateNumber,
                    PhoneNumber = registrationRecord.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing car registration");
                return StatusCode(500, new CarRegistrationResponse
                {
                    Success = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau"
                });
            }
        }
    }
}

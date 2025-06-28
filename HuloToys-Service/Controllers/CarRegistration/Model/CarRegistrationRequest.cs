namespace HuloToys_Service.Controllers.CarRegistration.Model
{
    public class CarRegistrationRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class CarRegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? QueueNumber { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public string? PlateNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RemainingTimeMinutes { get; set; }
    }

    public class RegistrationRecord
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public int QueueNumber { get; set; }
        public DateTime RegistrationTime { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class TimeRestrictionResult
    {
        public bool CanSubmit { get; set; }
        public int RemainingMinutes { get; set; }
        public DateTime? LastSubmission { get; set; }
    }
}

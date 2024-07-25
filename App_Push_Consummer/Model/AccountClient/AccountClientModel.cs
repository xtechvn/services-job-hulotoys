namespace HuloToys_Service.Models
{
    public class AccountClientModel
    {
        public int Id { get; set; }
        public long? ClientId { get; set; }
        public int? ClientType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordBackup { get; set; }
        public string ForgotPasswordToken { get; set; }
        public byte? Status { get; set; }
        public int? GroupPermission { get; set; }
        public string ClientCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ClientName { get; set; }
        public byte? isReceiverInfoEmail { get; set; }
    }
}

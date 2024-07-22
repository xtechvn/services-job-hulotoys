using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Mail
{
    public class AccountClient
    {
        public int id { get; set; }
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordBackup { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int ClientType { get; set; }
        public int AccountId { get; set; }
        public int? GroupPermission { get; set; }
    }
}

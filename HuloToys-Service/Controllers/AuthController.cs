// Create by: cuonglv
// Create date: 24-07-2024
using HuloToys_Service.Models;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
namespace HuloToys_Service.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel user)
        {
            try
            {                
                // Kiểm tra thông tin đăng nhập
                // Check trong ES xem user này có tồn tại hay không ?
                // đẩy data từ CMS lên Elasticsearch node accountAccessApi. Sau đó truy vấn lấy thông tin từ đó xuống
                if (user.Username == "test" && user.Password == "password")
                {
                    var token = GenerateJwtToken(user.Username);
                    return Ok(new { token });
                }
                else
                {
                    return Ok(new { msg="Thông tin đăng nhập không hợp lệ" });
                }
                
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new { msg = "Thông tin đăng nhập không hợp lệ. Vui lòng liên hệ với Admin" });
            }
        }

        private string GenerateJwtToken(string username)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return "";               
            }
        }
    }
}

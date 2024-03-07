using leaveSystem_vue.Data;
using leaveSystem_vue.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;



namespace leaveSystem_vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ApplicationDbContext context, IConfiguration configuration, ILogger<LoginController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.username == loginModel.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "用戶名不存在" });
            }
            else if (user.password != loginModel.Password)
            {
                return Unauthorized(new { message = "密碼錯誤" });
            }

            var token = GenerateJwtToken(user);
            // 創建 HttpOnly Cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            return Ok(new { Token = token, user.Role });// 可能還需要返回其他用戶信息
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/", // 確保這里和設置 cookie 時的路徑相匹配
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            return Ok("Logged out");
        }


        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // 嘗試從Cookie中獲取JWT Token
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var token))
            {
                return BadRequest("Token is missing");
            }

            var principal = GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                return BadRequest("Invalid token");
            }

            var username = principal.Identity.Name;
            var user = _context.Employees.FirstOrDefault(u => u.username == username);

            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            var newToken = GenerateJwtToken(user);
            // 更新Cookie中的JWT Token
            Response.Cookies.Append("jwt", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // 在生產環境中，應確保僅通過HTTPS發送Cookie
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { Token = newToken });
        }

        private string GenerateJwtToken(Employee user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.username),
                    // 根據需要添加更多的claims
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // 為了從過期的token中提取claims，這里設置為false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}

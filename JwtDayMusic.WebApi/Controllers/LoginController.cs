using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.LoginServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginDto loginDto)
        {
            var token = await _loginService.LoginAsync(loginDto);
            if (token is null)
            {
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });
            }
            return Ok(new { token });
        }
    }
}

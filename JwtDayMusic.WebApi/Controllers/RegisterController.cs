using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.RegisterService;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            await _registerService.RegisterAsync(registerDto);
            return Ok("İşlem başarılı");
        }
    }
}

using System.Security.Claims;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.ProfileServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var value = await _profileService.GetProfileAsync(userId);
            if (value == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            return Ok(value);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Surname))
                return BadRequest(new { message = "Ad ve soyad boş olamaz." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ok = await _profileService.UpdateProfileAsync(userId, dto);

            if (!ok)
                return BadRequest(new { message = "Güncelleme başarısız." });

            return Ok(new { message = "Profil güncellendi." });
        }
    }
}
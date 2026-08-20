using System.Security.Claims;
using JwtDayMusic.WebApi.Enums;
using JwtDayMusic.WebApi.Helpers;
using JwtDayMusic.WebApi.Services.SongServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        // Tüm şarkıları herkes görebilir.
        [HttpGet]
        public async Task<IActionResult> SongList()
        {
            var values = await _songService.GetAllSongsAsync();
            return Ok(values);
        }

        [Authorize]
        [HttpGet("Play/{id}")]
        public async Task<IActionResult> Play(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userTier = User.FindAll(ClaimTypes.Role)
                .Select(c => TierHelper.ToTierValue(c.Value))
                .Where(v => v > 0)
                .DefaultIfEmpty(1)
                .Max();

            var result = await _songService.GetPlayInfoAsync(id, userTier);

            if (!result.Found)
                return NotFound(new { message = "Şarkı bulunamadı." });

            if (!result.Allowed)
                return StatusCode(403, new
                {
                    message = $"Bu şarkı için en az '{(MembershipTier)result.RequiredTier}' paketi gerekiyor.",
                    requiredTier = result.RequiredTier
                });

            // İzin verildi → dinleme geçmişine kaydet (ML için veri birikiyor).
            await _songService.RecordListenAsync(userId, id);

            return Ok(new { title = result.Title, audioUrl = result.AudioUrl });
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string? q)
        {
            var values = await _songService.SearchAsync(q ?? "");
            return Ok(values);
        }
    }
}
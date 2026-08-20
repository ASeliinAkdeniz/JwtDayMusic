using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.ArtistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        // Singer bölümü herkese açık — [Authorize] YOK.
        [HttpGet]
        public async Task<IActionResult> ArtistList()
        {
            var values = await _artistService.GetAllArtists();
            return Ok(values);
        }

        // Tek artistin detayı + o artiste ait tüm şarkılar.
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var value = await _artistService.GetArtistDetailAsync(id);
            if (value == null)
                return NotFound(new { message = "Sanatçı bulunamadı." });
            return Ok(value);
        }

        [HttpPost]
        [Authorize(Roles = "Gold")]
        public async Task<IActionResult> CreateArtist(CreateArtistDto createArtistDto)
        {
            await _artistService.CreateArtistAsync(createArtistDto);
            return Ok("İşlem Başarılı");
        }
    }
}
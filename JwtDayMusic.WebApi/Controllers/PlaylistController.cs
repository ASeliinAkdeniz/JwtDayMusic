using System.Security.Claims;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.PlaylistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Authorize]   // playlist kişiye özel → giriş şart
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost]
        public async Task<IActionResult> Create(CreatePlaylistDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Playlist adı boş olamaz." });

            var result = await _playlistService.CreateAsync(GetUserId(), dto.Name);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> MyPlaylists()
        {
            var values = await _playlistService.GetMyPlaylistsAsync(GetUserId());
            return Ok(values);
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var value = await _playlistService.GetDetailAsync(GetUserId(), id);
            if (value == null)
                return NotFound(new { message = "Playlist bulunamadı." });
            return Ok(value);
        }

        [HttpPost("AddSong")]
        public async Task<IActionResult> AddSong(PlaylistSongDto dto)
        {
            var ok = await _playlistService.AddSongAsync(GetUserId(), dto.PlaylistId, dto.SongId);
            if (!ok)
                return BadRequest(new { message = "Şarkı eklenemedi." });
            return Ok(new { message = "Şarkı eklendi." });
        }

        [HttpPost("RemoveSong")]
        public async Task<IActionResult> RemoveSong(PlaylistSongDto dto)
        {
            var ok = await _playlistService.RemoveSongAsync(GetUserId(), dto.PlaylistId, dto.SongId);
            if (!ok)
                return BadRequest(new { message = "Şarkı çıkarılamadı." });
            return Ok(new { message = "Şarkı çıkarıldı." });
        }
    }
}
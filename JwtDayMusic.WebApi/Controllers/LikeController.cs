using System.Security.Claims;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.LikeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Authorize]   // beğeni kişiye özel → giriş şart
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost("Toggle")]
        public async Task<IActionResult> Toggle(LikeToggleDto dto)
        {
            var result = await _likeService.ToggleAsync(GetUserId(), dto.SongId);
            return Ok(result);
        }

        [HttpGet("MyLikes")]
        public async Task<IActionResult> MyLikes()
        {
            var values = await _likeService.GetMyLikesAsync(GetUserId());
            return Ok(values);
        }

        [HttpGet("LikedIds")]
        public async Task<IActionResult> LikedIds()
        {
            var values = await _likeService.GetLikedSongIdsAsync(GetUserId());
            return Ok(values);
        }
    }
}
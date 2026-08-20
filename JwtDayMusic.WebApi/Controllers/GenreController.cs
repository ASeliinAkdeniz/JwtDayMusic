using JwtDayMusic.WebApi.Services.GenreServices;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // Türler herkese açık — [Authorize] YOK.
        [HttpGet]
        public async Task<IActionResult> GenreList()
        {
            var values = await _genreService.GetAllGenres();
            return Ok(values);
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var value = await _genreService.GetGenreDetailAsync(id);
            if (value == null)
                return NotFound(new { message = "Tür bulunamadı." });
            return Ok(value);
        }
    }
}
using JwtDayMusic.WebApi.Services.ImportServices;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        // Tetikleyince iTunes'tan çeker. (Tek seferlik kullanım için; auth koymadım.)
        [HttpPost("Itunes")]
        public async Task<IActionResult> Itunes()
        {
            var count = await _importService.ImportFromItunesAsync();
            return Ok(new { message = $"{count} şarkı içe aktarıldı." });
        }
    }
}
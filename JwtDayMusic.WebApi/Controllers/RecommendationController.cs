using System.Security.Claims;
using JwtDayMusic.WebApi.Services.RecommendationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Authorize]   // öneri kişiye özel → giriş şart
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<IActionResult> MyRecommendations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var values = await _recommendationService.GetRecommendationsAsync(userId);
            return Ok(values);
        }
    }
}
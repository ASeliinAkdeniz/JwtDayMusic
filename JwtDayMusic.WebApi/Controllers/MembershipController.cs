using System.Security.Claims;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Services.MembershipServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Authorize]   // sadece giriş şart; rol koşulu YOK, o yüzden çakışma olmaz
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet("Current")]
        public async Task<IActionResult> Current()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tier = await _membershipService.GetCurrentTierAsync(userId);
            return Ok(new { tier });
        }

        [HttpPost("Buy")]
        public async Task<IActionResult> Buy([FromBody] BuyMembershipDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _membershipService.BuyAsync(userId, dto.Tier);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, tier = result.Tier, token = result.Token });
        }
    }
}
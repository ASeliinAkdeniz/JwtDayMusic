using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string rolename)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = rolename
            };
            await _roleManager.CreateAsync(identityRole);
            return Ok("Başarılı");
        }
    }
}

using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using Microsoft.AspNetCore.Identity;

namespace JwtDayMusic.WebApi.Services.RegisterService
{
    public class RegisterService: IRegisterService      
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            AppUser appUser = new AppUser()
            {
                Email = dto.Email,
                UserName = dto.Username,
                Name = dto.Name,
                Surname = dto.Surname
            };
            var result = await _userManager.CreateAsync(appUser, dto.Password);
            return result.Succeeded;
        }
    }
}

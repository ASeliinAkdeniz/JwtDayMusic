using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.RegisterService
{
    public interface IRegisterService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
    }
}

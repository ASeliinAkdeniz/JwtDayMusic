using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.ProfileServices
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}
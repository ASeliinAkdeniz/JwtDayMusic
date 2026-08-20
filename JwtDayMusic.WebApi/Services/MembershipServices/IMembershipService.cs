using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.MembershipServices
{
    public interface IMembershipService
    {
        Task<MembershipResultDto> BuyAsync(string userId, string tierName);
        Task<string> GetCurrentTierAsync(string userId);
    }
}
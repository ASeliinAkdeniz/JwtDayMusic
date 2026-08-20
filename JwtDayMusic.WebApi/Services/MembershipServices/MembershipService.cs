using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using JwtDayMusic.WebApi.Enums;
using JwtDayMusic.WebApi.Services.LoginServices;
using Microsoft.AspNetCore.Identity;

namespace JwtDayMusic.WebApi.Services.MembershipServices
{
    public class MembershipService : IMembershipService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILoginService _loginService;

        public MembershipService(UserManager<AppUser> userManager, ILoginService loginService)
        {
            _userManager = userManager;
            _loginService = loginService;
        }

        public async Task<MembershipResultDto> BuyAsync(string userId, string tierName)
        {
            // Geçerli bir kademe adı mı? ("Gold" gibi)
            if (!Enum.TryParse<MembershipTier>(tierName, out var tier))
                return new MembershipResultDto { Success = false, Message = "Geçersiz paket." };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new MembershipResultDto { Success = false, Message = "Kullanıcı bulunamadı." };

            // Kullanıcı tek bir kademe tutsun: önce eski kademe rollerini temizle.
            var tierRoleNames = Enum.GetNames<MembershipTier>(); // Basic, Gold, Premium, Elit
            var currentRoles = await _userManager.GetRolesAsync(user);
            var toRemove = currentRoles.Where(r => tierRoleNames.Contains(r)).ToList();
            if (toRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, toRemove);

            // Yeni kademeyi ata.
            await _userManager.AddToRoleAsync(user, tier.ToString());

            // KRİTİK: rol değişti ama eski token hâlâ eski kademeyi taşır.
            // O yüzden GÜNCEL rollerle taze bir token üretiyoruz.
            var token = await _loginService.CreateTokenAsync(user);

            return new MembershipResultDto
            {
                Success = true,
                Message = $"{tier} paketi etkinleştirildi.",
                Tier = tier.ToString(),
                Token = token
            };
        }

        public async Task<string> GetCurrentTierAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "Basic";

            var tierRoleNames = Enum.GetNames<MembershipTier>();
            var roles = await _userManager.GetRolesAsync(user);
            var tierRoles = roles.Where(r => tierRoleNames.Contains(r)).ToList();

            if (!tierRoles.Any()) return "Basic";   // rolsüz = Basic

            // birden fazla varsa en yükseği
            return tierRoles
                .OrderByDescending(r => (int)Enum.Parse<MembershipTier>(r))
                .First();
        }
    }
}
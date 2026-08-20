using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using JwtDayMusic.WebApi.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.ProfileServices
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtContext _context;

        public ProfileService(UserManager<AppUser> userManager, JwtContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            // Kullanıcının en yüksek kademesi (rolsüz = Basic).
            var tierRoleNames = Enum.GetNames<MembershipTier>();
            var roles = await _userManager.GetRolesAsync(user);
            var tierRoles = roles.Where(r => tierRoleNames.Contains(r)).ToList();
            string tier = tierRoles.Any()
                ? tierRoles.OrderByDescending(r => (int)Enum.Parse<MembershipTier>(r)).First()
                : "Basic";

            // İstatistikler — CountAsync ile veritabanında sayılır (veri çekilmez).
            int playlistCount = await _context.Playlists.CountAsync(p => p.UserId == userId);
            int likedCount = await _context.SongLikes.CountAsync(l => l.UserId == userId);
            int listenCount = await _context.ListeningHistories.CountAsync(h => h.UserId == userId);

            return new ProfileDto
            {
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                Tier = tier,
                PlaylistCount = playlistCount,
                LikedCount = likedCount,
                ListenCount = listenCount
            };
        }
        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.ImageUrl = dto.ImageUrl;   // WebUI, yeni veya mevcut yolu gönderecek

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
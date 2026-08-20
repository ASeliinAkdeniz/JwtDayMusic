using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.LikeServices
{
    public class LikeService : ILikeService
    {
        private readonly JwtContext _context;

        public LikeService(JwtContext context)
        {
            _context = context;
        }

        public async Task<LikeResultDto> ToggleAsync(string userId, int songId)
        {
            var existing = await _context.SongLikes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.SongId == songId);

            if (existing != null)
            {
                // Zaten beğenmiş → geri al
                _context.SongLikes.Remove(existing);
                await _context.SaveChangesAsync();
                return new LikeResultDto { Liked = false, Message = "Beğeni geri alındı." };
            }

            // Beğenmemiş → önce şarkı gerçekten var mı?
            bool songExists = await _context.Songs.AnyAsync(s => s.SongId == songId);
            if (!songExists)
                return new LikeResultDto { Liked = false, Message = "Şarkı bulunamadı." };

            await _context.SongLikes.AddAsync(new SongLike
            {
                UserId = userId,
                SongId = songId,
                LikedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return new LikeResultDto { Liked = true, Message = "Beğenildi." };
        }

        public async Task<List<ResultSongDto>> GetMyLikesAsync(string userId)
        {
            return await _context.SongLikes
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.LikedDate)
                .Select(l => new ResultSongDto
                {
                    SongId = l.Song.SongId,
                    Title = l.Song.Title,
                    CoverImageUrl = l.Song.CoverImageUrl,
                    AudioUrl = l.Song.AudioUrl,
                    Duration = l.Song.Duration,
                    PlayCount = l.Song.PlayCount,
                    Tier = l.Song.Tier,
                    ReleaseDate = l.Song.ReleaseDate,
                    ArtistId = l.Song.ArtistId,
                    ArtistName = l.Song.Artist.Name,
                    GenreId = l.Song.GenreId,
                    GenreName = l.Song.Genre.Name
                })
                .ToListAsync();
        }

        public async Task<List<int>> GetLikedSongIdsAsync(string userId)
        {
            return await _context.SongLikes
                .Where(l => l.UserId == userId)
                .Select(l => l.SongId)
                .ToListAsync();
        }
    }
}
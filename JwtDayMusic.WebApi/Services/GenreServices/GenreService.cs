using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.GenreServices
{
    public class GenreService : IGenreService
    {
        private readonly JwtContext _context;

        public GenreService(JwtContext context)
        {
            _context = context;
        }

        public async Task<List<ResultGenreDto>> GetAllGenres()
        {
            return await _context.Genres
                .Select(g => new ResultGenreDto
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    ImageUrl = g.ImageUrl,
                    SongCount = g.Songs.Count   // EF bunu SQL'e çevirir
                })
                .ToListAsync();
        }

        public async Task<GenreDetailDto?> GetGenreDetailAsync(int id)
        {
            // Türü, şarkılarıyla ve şarkıların sanatçılarıyla birlikte yükle.
            var genre = await _context.Genres
                .Include(g => g.Songs)
                    .ThenInclude(s => s.Artist)
                .FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null) return null;

            return new GenreDetailDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                ImageUrl = genre.ImageUrl,
                Songs = genre.Songs.Select(s => new ResultSongDto
                {
                    SongId = s.SongId,
                    Title = s.Title,
                    CoverImageUrl = s.CoverImageUrl,
                    AudioUrl = s.AudioUrl,
                    Duration = s.Duration,
                    PlayCount = s.PlayCount,
                    Tier = s.Tier,
                    ReleaseDate = s.ReleaseDate,
                    ArtistId = s.ArtistId,
                    ArtistName = s.Artist.Name,
                    GenreId = genre.GenreId,
                    GenreName = genre.Name
                }).ToList()
            };
        }
    }
}
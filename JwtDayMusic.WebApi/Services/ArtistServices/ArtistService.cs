using AutoMapper;
using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.ArtistServices
{
    public class ArtistService : IArtistService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;
        public ArtistService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateArtistAsync(CreateArtistDto createArtistDto)
        {
         var value = _mapper.Map<Artist>(createArtistDto);
            await _context.Artists.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ResultArtistDto>> GetAllArtists()
        {
            var values = await _context.Artists.ToListAsync();
            return _mapper.Map<List<ResultArtistDto>>(values);
        }
        public async Task<ArtistDetailDto?> GetArtistDetailAsync(int id)
        {
            // Artisti, şarkılarıyla ve şarkıların türleriyle birlikte yükle.
            var artist = await _context.Artists
                .Include(a => a.Songs)
                    .ThenInclude(s => s.Genre)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null) return null;

            return new ArtistDetailDto
            {
                ArtistId = artist.ArtistId,
                Name = artist.Name,
                ImageUrl = artist.ImageUrl,
                Bio = artist.Bio,
                MonthlyListeners = artist.MonthlyListeners,
                IsVerified = artist.IsVerified,
                Songs = artist.Songs.Select(s => new ResultSongDto
                {
                    SongId = s.SongId,
                    Title = s.Title,
                    CoverImageUrl = s.CoverImageUrl,
                    AudioUrl = s.AudioUrl,
                    Duration = s.Duration,
                    PlayCount = s.PlayCount,
                    Tier = s.Tier,
                    ReleaseDate = s.ReleaseDate,
                    ArtistId = artist.ArtistId,
                    ArtistName = artist.Name,
                    GenreId = s.GenreId,
                    GenreName = s.Genre.Name
                }).ToList()
            };
        }
    }
}

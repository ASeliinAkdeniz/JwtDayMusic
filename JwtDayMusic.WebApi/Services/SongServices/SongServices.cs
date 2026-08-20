using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly JwtContext _context;

        public SongService(JwtContext context)
        {
            _context = context;
        }

        public async Task<List<ResultSongDto>> GetAllSongsAsync()
        {
            // Include: artist ve genre'yi de yükle ki adlarını okuyabilelim.
            // Burada AutoMapper yerine bilerek elle map yapıyorum; çünkü ilişkili
            // alanları (ArtistName, GenreName) düzleştiriyoruz ve bu daha okunur.
            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .OrderByDescending(s => s.ReleaseDate)
                .Select(s => new ResultSongDto
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
                    GenreId = s.GenreId,
                    GenreName = s.Genre.Name
                })
                .ToListAsync();

            return songs;
        }
        public async Task<PlaySongResultDto> GetPlayInfoAsync(int songId, int userTier)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.SongId == songId);

            if (song == null)
                return new PlaySongResultDto { Found = false };

            int songTier = (int)song.Tier;          // enum -> sayı (Basic=1 … Elit=4)
            bool allowed = userTier >= songTier;     // HİYERARŞİ: kullanıcı kademesi ≥ şarkı kademesi

            return new PlaySongResultDto
            {
                Found = true,
                Allowed = allowed,
                Title = song.Title,
                RequiredTier = songTier,
                AudioUrl = allowed ? song.AudioUrl : null   // izin yoksa linki VERME
            };
        }
        public async Task RecordListenAsync(string userId, int songId)
        {
            var history = new ListeningHistory
            {
                UserId = userId,
                SongId = songId,
                ListenedAt = DateTime.Now
            };
            await _context.ListeningHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ResultSongDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<ResultSongDto>();

            // Turkish-I güvenli arama:
            // ToLower() Türkçe kültürde "I → ı" yaptığı için karşılaştırmayı bozar.
            // Bu yüzden hem veriyi hem sorguyu ToLowerInvariant ile normalize ediyoruz.
            // Not: ToLowerInvariant, kültürden bağımsız (invariant) küçültme yapar.
            var q = query.Trim().ToLowerInvariant();

            // Önce ilgili tüm veriyi (Include ile) çekiyoruz, sonra bellekte filtreliyoruz.
            // Küçük katalog için bu yeterli ve kültür sorununu kesin çözer.
            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .ToListAsync();

            var result = songs
                .Where(s =>
                    s.Title.ToLowerInvariant().Contains(q) ||
                    s.Artist.Name.ToLowerInvariant().Contains(q) ||
                    s.Genre.Name.ToLowerInvariant().Contains(q))
                .Select(s => new ResultSongDto
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
                    GenreId = s.GenreId,
                    GenreName = s.Genre.Name
                })
                .ToList();

            return result;
        }
    }
}
using System.Net.Http.Json;
using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using JwtDayMusic.WebApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.ImportServices
{
    public class ImportService : IImportService
    {
        private readonly JwtContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImportService(JwtContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // (arama terimi, atanacak tür, kademe) — hazır liste
        private static readonly (string term, string genre, MembershipTier tier)[] Searches =
        {
            ("The Midnight synthwave", "Synthwave", MembershipTier.Gold),
            ("Kavinsky",               "Synthwave", MembershipTier.Premium),
            ("FM-84",                  "Synthwave", MembershipTier.Basic),
            ("Gunship synthwave",      "Synthwave", MembershipTier.Elit),
            ("Daft Punk",              "Elektronik", MembershipTier.Gold),
            ("The Weeknd",             "Pop",       MembershipTier.Basic),
            ("Dua Lipa",               "Pop",       MembershipTier.Basic),
            ("Depeche Mode",           "Rock",      MembershipTier.Premium),
        };

        public async Task<int> ImportFromItunesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            // Türleri önceden hazırla (varsa bul, yoksa oluştur).
            var genreCache = await _context.Genres.ToDictionaryAsync(g => g.Name, g => g);

            async Task<Genre> GetGenre(string name)
            {
                if (genreCache.TryGetValue(name, out var g)) return g;
                g = new Genre { Name = name, ImageUrl = null };
                _context.Genres.Add(g);
                await _context.SaveChangesAsync();
                genreCache[name] = g;
                return g;
            }

            int added = 0;

            foreach (var s in Searches)
            {
                var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(s.term)}&entity=song&limit=6";

                ItunesResponse? data;
                try
                {
                    data = await client.GetFromJsonAsync<ItunesResponse>(url);
                }
                catch
                {
                    continue; // bir arama başarısız olursa diğerlerine devam et
                }

                if (data?.results == null) continue;

                var genre = await GetGenre(s.genre);

                foreach (var t in data.results)
                {
                    // Önizlemesi olmayanları atla (çalamayız).
                    if (string.IsNullOrEmpty(t.previewUrl) || string.IsNullOrEmpty(t.trackName))
                        continue;

                    // Aynı şarkı zaten var mı? (aynı başlık + önizleme URL)
                    bool exists = await _context.Songs.AnyAsync(x => x.AudioUrl == t.previewUrl);
                    if (exists) continue;

                    // Sanatçı: aynı isim varsa onu kullan, yoksa oluştur.
                    var artist = await _context.Artists
                        .FirstOrDefaultAsync(a => a.Name == t.artistName);
                    if (artist == null)
                    {
                        artist = new Artist
                        {
                            Name = t.artistName,
                            ImageUrl = t.artworkUrl100,   // sanatçı görseli olarak kapak
                            Bio = $"{t.artistName} — iTunes üzerinden içe aktarıldı.",
                            MonthlyListeners = new Random().Next(100000, 2000000),
                            IsVerified = true,
                            CreatedDate = DateTime.Now
                        };
                        _context.Artists.Add(artist);
                        await _context.SaveChangesAsync();
                    }

                    // Kapağı büyüt: 100x100 yerine 300x300.
                    var cover = t.artworkUrl100?.Replace("100x100", "300x300") ?? t.artworkUrl100;

                    var duration = t.trackTimeMillis > 0
                        ? TimeSpan.FromMilliseconds(t.trackTimeMillis)
                        : TimeSpan.FromSeconds(30);

                    _context.Songs.Add(new Song
                    {
                        Title = t.trackName,
                        CoverImageUrl = cover,
                        AudioUrl = t.previewUrl,     // 30 sn önizleme mp3 (tam URL)
                        Duration = duration,
                        PlayCount = new Random().Next(5000, 90000),
                        Tier = s.tier,
                        ReleaseDate = DateTime.Now,
                        Artist = artist,
                        Genre = genre
                    });
                    added++;
                }

                await _context.SaveChangesAsync();
            }

            return added;
        }
    }
}
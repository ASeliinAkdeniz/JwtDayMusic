using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Entites;
using JwtDayMusic.WebApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(JwtContext context)
        {
            // Zaten şarkı varsa tekrar eklemiyoruz (idempotent).
            if (await context.Songs.AnyAsync())
                return;

            // --- Türler (Genre) ---
            var pop = new Genre { Name = "Pop", ImageUrl = null };
            var rock = new Genre { Name = "Rock", ImageUrl = null };
            var jazz = new Genre { Name = "Jazz", ImageUrl = null };
            var elektronik = new Genre { Name = "Elektronik", ImageUrl = null };

            // --- Sanatçılar (Artist) ---
            var neonVadi = new Artist
            {
                Name = "Neon Vadi",
                ImageUrl = "/images/artists/neon-vadi.jpg",
                Bio = "Sentetik seslerle modern pop üreten bir proje.",
                MonthlyListeners = 1250000,
                IsVerified = true,
                CreatedDate = DateTime.Now
            };
            var gokyuzu = new Artist
            {
                Name = "Gökyüzü Orkestrası",
                ImageUrl = "/images/artists/gokyuzu.jpg",
                Bio = "Rock ve caz füzyonunu bir araya getiren topluluk.",
                MonthlyListeners = 640000,
                IsVerified = true,
                CreatedDate = DateTime.Now
            };
            var denizYildiz = new Artist
            {
                Name = "Deniz Yıldız",
                ImageUrl = "/images/artists/deniz-yildiz.jpg",
                Bio = "Elektronik müzik prodüktörü ve DJ.",
                MonthlyListeners = 320000,
                IsVerified = false,
                CreatedDate = DateTime.Now
            };

            // --- Şarkılar (Song) ---
            // Navigation property (Artist/Genre) atadığımız için EF, ArtistId/GenreId'yi
            // kendisi dolduruyor; ayrıca elle Id vermemize gerek yok.
            // AudioUrl'ler şimdilik placeholder; gerçek .mp3 bağlamayı Faz 5'te yapacağız.
            var songs = new List<Song>
            {
                new Song
                {
                    Title = "Gece Yarısı", CoverImageUrl = "/images/covers/gece-yarisi.jpg",
                    AudioUrl = "/audio/gece-yarisi.mp3", Duration = TimeSpan.FromSeconds(198),
                    PlayCount = 54000, Tier = MembershipTier.Basic,
                    ReleaseDate = new DateTime(2024, 3, 12), Artist = neonVadi, Genre = pop
                },
                new Song
                {
                    Title = "Şehrin Işıkları", CoverImageUrl = "/images/covers/sehrin-isiklari.jpg",
                    AudioUrl = "/audio/sehrin-isiklari.mp3", Duration = TimeSpan.FromSeconds(224),
                    PlayCount = 31000, Tier = MembershipTier.Gold,
                    ReleaseDate = new DateTime(2024, 6, 1), Artist = neonVadi, Genre = pop
                },
                new Song
                {
                    Title = "Fırtına Öncesi", CoverImageUrl = "/images/covers/firtina-oncesi.jpg",
                    AudioUrl = "/audio/firtina-oncesi.mp3", Duration = TimeSpan.FromSeconds(305),
                    PlayCount = 18700, Tier = MembershipTier.Premium,
                    ReleaseDate = new DateTime(2023, 11, 20), Artist = gokyuzu, Genre = rock
                },
                new Song
                {
                    Title = "Sonsuz Yol", CoverImageUrl = "/images/covers/sonsuz-yol.jpg",
                    AudioUrl = "/audio/sonsuz-yol.mp3", Duration = TimeSpan.FromSeconds(276),
                    PlayCount = 9800, Tier = MembershipTier.Elit,
                    ReleaseDate = new DateTime(2025, 1, 8), Artist = gokyuzu, Genre = jazz
                },
                new Song
                {
                    Title = "Mavi Saat", CoverImageUrl = "/images/covers/mavi-saat.jpg",
                    AudioUrl = "/audio/mavi-saat.mp3", Duration = TimeSpan.FromSeconds(241),
                    PlayCount = 42000, Tier = MembershipTier.Basic,
                    ReleaseDate = new DateTime(2024, 9, 15), Artist = denizYildiz, Genre = elektronik
                },
                new Song
                {
                    Title = "Derin Frekans", CoverImageUrl = "/images/covers/derin-frekans.jpg",
                    AudioUrl = "/audio/derin-frekans.mp3", Duration = TimeSpan.FromSeconds(312),
                    PlayCount = 15600, Tier = MembershipTier.Gold,
                    ReleaseDate = new DateTime(2025, 2, 22), Artist = denizYildiz, Genre = elektronik
                },
                new Song
                {
                    Title = "Kırık Cam", CoverImageUrl = "/images/covers/kirik-cam.jpg",
                    AudioUrl = "/audio/kirik-cam.mp3", Duration = TimeSpan.FromSeconds(263),
                    PlayCount = 7200, Tier = MembershipTier.Premium,
                    ReleaseDate = new DateTime(2023, 5, 3), Artist = gokyuzu, Genre = rock
                }
            };

            await context.Songs.AddRangeAsync(songs);
            await context.SaveChangesAsync();
        }
    }
}
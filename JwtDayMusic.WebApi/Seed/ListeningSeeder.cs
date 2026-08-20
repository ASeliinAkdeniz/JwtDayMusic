using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Entites;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Seed
{
    public static class ListeningSeeder
    {
        public static async Task SeedAsync(JwtContext context)
        {
            // Sahte veri zaten varsa tekrar ekleme.
            bool alreadySeeded = await context.ListeningHistories
                .AnyAsync(h => h.UserId.StartsWith("seed-"));
            if (alreadySeeded) return;

            // Veritabanındaki GERÇEK şarkı Id'lerini al (sabit 1..7 varsaymıyoruz).
            var songIds = await context.Songs
                .OrderBy(s => s.SongId)
                .Select(s => s.SongId)
                .Take(7)
                .ToListAsync();

            // Yeterli şarkı yoksa seed yapma (foreign key hatası olmasın).
            if (songIds.Count < 6) return;

            // Sahte kullanıcılar, mevcut şarkı Id'leri üzerinden örüntülü dinleme.
            // (indeksler: 0,1 -> ilk iki şarkı; 2,3 -> sonraki; vb.)
            var patterns = new Dictionary<string, int[]>
            {
                ["seed-pop-1"] = new[] { songIds[0], songIds[1] },
                ["seed-pop-2"] = new[] { songIds[0], songIds[1], songIds[4] },
                ["seed-rock-1"] = new[] { songIds[2], songIds[songIds.Count - 1], songIds[3] },
                ["seed-rock-2"] = new[] { songIds[2], songIds[songIds.Count - 1] },
                ["seed-elec-1"] = new[] { songIds[4], songIds[5] },
                ["seed-elec-2"] = new[] { songIds[4], songIds[5], songIds[0] },
            };

            var rows = new List<ListeningHistory>();
            foreach (var kv in patterns)
            {
                foreach (var songId in kv.Value)
                {
                    rows.Add(new ListeningHistory
                    {
                        UserId = kv.Key,
                        SongId = songId,
                        ListenedAt = DateTime.Now
                    });
                }
            }

            await context.ListeningHistories.AddRangeAsync(rows);
            await context.SaveChangesAsync();
        }
    }
}
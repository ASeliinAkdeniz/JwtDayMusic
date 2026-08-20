using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.ML;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace JwtDayMusic.WebApi.Services.RecommendationServices
{
    public class RecommendationService : IRecommendationService
    {
        private readonly JwtContext _context;

        public RecommendationService(JwtContext context)
        {
            _context = context;
        }

        public async Task<List<ResultSongDto>> GetRecommendationsAsync(string userId, int count = 4)
        {
            // 1) Tüm dinleme geçmişini çek (sahte seed + gerçek kullanıcılar).
            var history = await _context.ListeningHistories
                .Select(h => new { h.UserId, h.SongId })
                .ToListAsync();

            // Yeterli veri yoksa öneri üretme (model eğitilemez).
            if (history.Count < 5)
                return new List<ResultSongDto>();

            // 2) String UserId'leri sayıya (uint) çevir. ML.NET key olarak sayı ister.
            var userMap = history.Select(h => h.UserId).Distinct()
                                 .Select((id, i) => new { id, idx = (uint)(i + 1) })
                                 .ToDictionary(x => x.id, x => x.idx);

            // Bu kullanıcı hiç dinlememişse öneri yok (soğuk başlangıç).
            if (!userMap.ContainsKey(userId))
                return new List<ResultSongDto>();

            var trainRows = history.Select(h => new ListeningData
            {
                UserIdEncoded = userMap[h.UserId],
                SongIdEncoded = (uint)h.SongId,
                Label = 1f
            }).ToList();

            // 3) ML.NET pipeline: Matrix Factorization eğit.
            var mlContext = new MLContext();
            var trainingData = mlContext.Data.LoadFromEnumerable(trainRows);

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(ListeningData.UserIdEncoded),
                MatrixRowIndexColumnName = nameof(ListeningData.SongIdEncoded),
                LabelColumnName = nameof(ListeningData.Label),
                NumberOfIterations = 20,
                ApproximationRank = 8,
                // "sadece pozitif sinyal var" senaryosu için uygun kayıp fonksiyonu:
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
                C = 0.00001
            };

            var estimator = mlContext.Recommendation().Trainers.MatrixFactorization(options);
            var model = estimator.Fit(trainingData);

            var predictionEngine = mlContext.Model
                .CreatePredictionEngine<ListeningData, SongPrediction>(model);

            // 4) Kullanıcının HENÜZ dinlemediği şarkılar için skor tahmin et.
            var listenedSongIds = history.Where(h => h.UserId == userId)
                                         .Select(h => h.SongId).ToHashSet();

            var allSongIds = await _context.Songs.Select(s => s.SongId).ToListAsync();
            var candidateIds = allSongIds.Where(id => !listenedSongIds.Contains(id)).ToList();

            var scored = new List<(int songId, float score)>();
            foreach (var songId in candidateIds)
            {
                var p = predictionEngine.Predict(new ListeningData
                {
                    UserIdEncoded = userMap[userId],
                    SongIdEncoded = (uint)songId
                });
                scored.Add((songId, p.Score));
            }

            // 5) En yüksek skorlu N şarkıyı seç ve DTO'ya çevir.
            var topIds = scored.OrderByDescending(x => x.score)
                               .Take(count)
                               .Select(x => x.songId)
                               .ToList();

            if (!topIds.Any())
                return new List<ResultSongDto>();

            var songs = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Genre)
                .Where(s => topIds.Contains(s.SongId))
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
    }
}
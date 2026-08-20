using Microsoft.ML.Data;

namespace JwtDayMusic.WebApi.ML
{
    // Modele giren tek satır: bir kullanıcı bir şarkıyı dinledi.
    public class ListeningData
    {
        [KeyType(count: 100000)]
        public uint UserIdEncoded { get; set; }

        [KeyType(count: 100000)]
        public uint SongIdEncoded { get; set; }

        public float Label { get; set; }   // "dinledi" sinyali = 1
    }

    // Modelin çıkışı: tahmini skor.
    public class SongPrediction
    {
        public float Score { get; set; }
    }
}
using JwtDayMusic.WebApi.Enums;

namespace JwtDayMusic.WebApi.Entites
{
    public class Song
    {
        public int SongId { get; set; }

        public string Title { get; set; }

        public string CoverImageUrl { get; set; }

        public string AudioUrl { get; set; }   // .mp3 dosyasının yolu

        public TimeSpan Duration { get; set; }

        public long PlayCount { get; set; }

        // ESKİ: public bool IsPremium { get; set; }
        // YENİ: şarkının paket kademesi
        public MembershipTier Tier { get; set; }

        public DateTime ReleaseDate { get; set; }

        // Artist ilişkisi (mevcut)
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }

        // Genre (Tür) ilişkisi (yeni)
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
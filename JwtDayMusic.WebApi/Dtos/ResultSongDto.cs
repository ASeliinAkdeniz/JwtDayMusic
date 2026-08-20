using JwtDayMusic.WebApi.Enums;

namespace JwtDayMusic.WebApi.Dtos
{
    public class ResultSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public long PlayCount { get; set; }
        public MembershipTier Tier { get; set; }
        public DateTime ReleaseDate { get; set; }

        // İlişkili verilerden düz alanlar (kartta göstermek için)
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }

        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
namespace JwtDayMusic.WebUI.Dtos
{
    public class ResultSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public long PlayCount { get; set; }
        public int Tier { get; set; }   // 1=Basic, 2=Gold, 3=Premium, 4=Elit
        public DateTime ReleaseDate { get; set; }

        public int ArtistId { get; set; }
        public string ArtistName { get; set; }

        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
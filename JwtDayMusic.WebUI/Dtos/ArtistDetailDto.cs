namespace JwtDayMusic.WebUI.Dtos
{
    public class ArtistDetailDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
        public long MonthlyListeners { get; set; }
        public bool IsVerified { get; set; }

        public List<ResultSongDto> Songs { get; set; }
    }
}
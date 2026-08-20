namespace JwtDayMusic.WebUI.Dtos
{
    public class PlaylistDetailDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ResultSongDto> Songs { get; set; }
    }
}
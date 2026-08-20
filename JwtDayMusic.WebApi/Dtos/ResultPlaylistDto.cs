namespace JwtDayMusic.WebApi.Dtos
{
    public class ResultPlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SongCount { get; set; }
    }
}
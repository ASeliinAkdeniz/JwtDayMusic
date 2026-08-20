namespace JwtDayMusic.WebApi.Dtos
{
    public class GenreDetailDto
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }

        public List<ResultSongDto> Songs { get; set; }
    }
}
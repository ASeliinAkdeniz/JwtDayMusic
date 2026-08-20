namespace JwtDayMusic.WebApi.Entites
{
    public class Genre
    {
        public int GenreId { get; set; }

        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        // İlişki: bir türe ait birçok şarkı
        public List<Song> Songs { get; set; }
    }
}
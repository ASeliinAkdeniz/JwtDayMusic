namespace JwtDayMusic.WebApi.Entites
{
    public class Playlist
    {
        public int PlaylistId { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }   // sahibi (AppUser.Id — string)

        public DateTime CreatedDate { get; set; }

        // Bu playlist'teki şarkı bağlantıları
        public List<PlaylistSong> PlaylistSongs { get; set; }
    }
}
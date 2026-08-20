namespace JwtDayMusic.WebApi.Entites
{
    public class PlaylistSong
    {
        public int PlaylistSongId { get; set; }

        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
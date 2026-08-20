namespace JwtDayMusic.WebApi.Entites
{
    public class ListeningHistory
    {
        public int ListeningHistoryId { get; set; }

        public string UserId { get; set; }   // AppUser.Id (Identity string id)

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime ListenedAt { get; set; }
    }
}
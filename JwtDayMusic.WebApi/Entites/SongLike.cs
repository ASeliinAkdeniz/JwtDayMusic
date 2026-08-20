namespace JwtDayMusic.WebApi.Entites
{
    public class SongLike
    {
        public int SongLikeId { get; set; }

        public string UserId { get; set; }   // beğenen kullanıcı (AppUser.Id)

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime LikedDate { get; set; }
    }
}
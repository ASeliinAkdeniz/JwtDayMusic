namespace JwtDayMusic.WebApi.Dtos
{
    public class PlaySongResultDto
    {
        public bool Found { get; set; }        // şarkı var mı?
        public bool Allowed { get; set; }      // kullanıcının kademesi yetiyor mu?
        public string Title { get; set; }
        public int RequiredTier { get; set; }  // şarkının kademesi (403 mesajı için)
        public string? AudioUrl { get; set; }  // SADECE Allowed ise dolu; değilse null
    }
}
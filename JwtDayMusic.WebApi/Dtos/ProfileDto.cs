namespace JwtDayMusic.WebApi.Dtos
{
    public class ProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string Tier { get; set; }

        // İstatistikler
        public int PlaylistCount { get; set; }
        public int LikedCount { get; set; }
        public int ListenCount { get; set; }
    }
}
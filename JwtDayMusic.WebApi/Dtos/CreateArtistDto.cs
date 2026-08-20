namespace JwtDayMusic.WebApi.Dtos
{
    public class CreateArtistDto
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Bio { get; set; }

        public long MonthlyListeners { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}

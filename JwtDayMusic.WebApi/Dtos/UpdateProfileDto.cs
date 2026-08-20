namespace JwtDayMusic.WebApi.Dtos
{
    public class UpdateProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
    }
}
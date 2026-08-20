namespace JwtDayMusic.WebApi.Dtos
{
    public class MembershipResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Tier { get; set; }
        public string? Token { get; set; }   // yeni (taze) token
    }
}
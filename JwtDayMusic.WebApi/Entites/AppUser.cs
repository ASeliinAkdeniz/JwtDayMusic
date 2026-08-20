using Microsoft.AspNetCore.Identity;

namespace JwtDayMusic.WebApi.Entites
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
    }
}

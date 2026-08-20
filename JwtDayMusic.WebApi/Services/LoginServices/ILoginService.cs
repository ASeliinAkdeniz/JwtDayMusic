using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using System.Threading.Tasks;

namespace JwtDayMusic.WebApi.Services.LoginServices
{
    // "class" yerine "interface" kullanıyoruz. 
    public interface ILoginService
    {
        // C# isimlendirme standartlarına uyması için LoginAsync baş harfi büyük yazıldı
        // Giriş başarısızsa null döner; controller bu durumda 401 döndürmelidir.
        Task<string?> LoginAsync(LoginDto loginDto);
        Task<string> CreateTokenAsync(AppUser user);
    }
}
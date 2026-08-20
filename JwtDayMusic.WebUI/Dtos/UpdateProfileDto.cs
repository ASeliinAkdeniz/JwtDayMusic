using Microsoft.AspNetCore.Http;

namespace JwtDayMusic.WebUI.Dtos
{
    public class UpdateProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public string? CurrentImageUrl { get; set; }   // mevcut resim (değişmezse korunur)
        public IFormFile? ImageFile { get; set; }       // yeni yüklenen dosya (opsiyonel)
    }
}
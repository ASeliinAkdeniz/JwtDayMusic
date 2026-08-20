using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace JwtDayMusic.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Profile");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("SignIn", "Login");

            var json = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<ProfileDto>(json);
            return View(value);
        }
        // Düzenleme formunu göster — mevcut bilgileri doldurarak.
        public async Task<IActionResult> Edit()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Profile");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<ProfileDto>(json);

            var model = new UpdateProfileDto
            {
                Name = profile.Name,
                Surname = profile.Surname,
                CurrentImageUrl = profile.ImageUrl
            };
            return View(model);
        }

        // Formu kaydet — resmi diske yaz, yolu API'ye gönder.
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProfileDto model)
        {
            // Varsayılan: mevcut resmi koru.
            string? imageUrl = model.CurrentImageUrl;

            // Yeni bir dosya yüklendiyse wwwroot/images/profiles altına kaydet.
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(),
                                              "wwwroot", "images", "profiles");
                Directory.CreateDirectory(uploadsDir);   // yoksa oluştur

                // Çakışmasın diye benzersiz ad.
                var extension = Path.GetExtension(model.ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;
                var fullPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Tarayıcının erişeceği yol (wwwroot köküne göre).
                imageUrl = "/images/profiles/" + fileName;
            }

            // API'ye güncelleme isteği (token'ı handler ekliyor).
            var client = _httpClientFactory.CreateClient("ApiClient");
            var body = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    name = model.Name,
                    surname = model.Surname,
                    imageUrl = imageUrl
                }),
                Encoding.UTF8, "application/json");

            await client.PostAsync("api/Profile/Update", body);

            return RedirectToAction("Index");
        }
    }
}
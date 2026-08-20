using System.Text;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PlaylistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Playlist'lerim sayfası
        public async Task<IActionResult> Index()
        {
            // Giriş yoksa login'e
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Playlist");

            if (!response.IsSuccessStatusCode)
                return View(new List<ResultPlaylistDto>());

            var json = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultPlaylistDto>>(json)
                         ?? new List<ResultPlaylistDto>();
            return View(values);
        }

        // Yeni playlist oluştur (form gönderimi)
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var body = new StringContent(
                    JsonConvert.SerializeObject(new { name }),
                    Encoding.UTF8, "application/json");
                await client.PostAsync("api/Playlist", body);
            }
            return RedirectToAction("Index");
        }

        // Playlist detayı
        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Playlist/Detail/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<PlaylistDetailDto>(json);
            return View(value);
        }

        // Şarkı çıkar (detay sayfasındaki JS buraya gelir)
        [HttpPost]
        public async Task<IActionResult> RemoveSong(int playlistId, int songId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var body = new StringContent(
                JsonConvert.SerializeObject(new { playlistId, songId }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Playlist/RemoveSong", body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Response.StatusCode = 400;

            return Content(json, "application/json");
        }
        // Menü için: kullanıcının playlist'lerini sade JSON olarak döndürür.
        [HttpGet]
        public async Task<IActionResult> MyPlaylistsJson()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Playlist");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Response.StatusCode = (int)response.StatusCode;

            return Content(json, "application/json");
        }

        // Şarkı ekle
        [HttpPost]
        public async Task<IActionResult> AddSong(int playlistId, int songId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var body = new StringContent(
                JsonConvert.SerializeObject(new { playlistId, songId }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Playlist/AddSong", body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Response.StatusCode = 400;

            return Content(json, "application/json");
        }
    }
}
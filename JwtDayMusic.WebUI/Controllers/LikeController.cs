using System.Text;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class LikeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LikeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Favorilerim sayfası
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Like/MyLikes");

            if (!response.IsSuccessStatusCode)
                return View(new List<ResultSongDto>());

            var json = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultSongDto>>(json)
                         ?? new List<ResultSongDto>();
            return View(values);
        }

        // Tarayıcıdaki JS buraya gelir → API'ye token'lı iletir. (beğen/geri al)
        [HttpPost]
        public async Task<IActionResult> Toggle(int songId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var body = new StringContent(
                JsonConvert.SerializeObject(new { songId }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Like/Toggle", body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Response.StatusCode = (int)response.StatusCode;

            return Content(json, "application/json");
        }

        // Kalpleri başlangıçta doldurmak için: beğenilen şarkı id'leri.
        [HttpGet]
        public async Task<IActionResult> LikedIds()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Like/LikedIds");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                Response.StatusCode = (int)response.StatusCode;

            return Content(json, "application/json");
        }
    }
}
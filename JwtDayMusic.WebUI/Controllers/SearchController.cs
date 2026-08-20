using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class SearchController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SearchController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Navbar'daki arama kutusu buraya GET ile gelir: /Search?q=...
        public async Task<IActionResult> Index(string q)
        {
            ViewBag.Query = q;

            if (string.IsNullOrWhiteSpace(q))
                return View(new List<ResultSongDto>());

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Song/Search?q={Uri.EscapeDataString(q)}");

            if (!response.IsSuccessStatusCode)
                return View(new List<ResultSongDto>());

            var json = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultSongDto>>(json)
                         ?? new List<ResultSongDto>();
            return View(values);
        }
    }
}
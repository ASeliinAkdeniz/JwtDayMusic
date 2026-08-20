using System.Net;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class SongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Song");

            if (!response.IsSuccessStatusCode)
                return View(new List<ResultSongDto>());

            var jsonData = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultSongDto>>(jsonData)
                         ?? new List<ResultSongDto>();

            return View(values);
        }

        // Tarayıcıdaki JS buraya istek atar; token'ı JwtAuthorizationHandler otomatik ekler.
        [HttpGet]
        public async Task<IActionResult> Play(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Song/Play/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var json = await response.Content.ReadAsStringAsync();
                Response.StatusCode = 403;
                return Content(json, "application/json");
            }

            Response.StatusCode = 401;
            return Content("{\"message\":\"Oynatmak için giriş yapmalısınız.\"}", "application/json");
        }
        public async Task<IActionResult> Recommendations()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Recommendation");

            if (!response.IsSuccessStatusCode)
                return PartialView("_Recommendations", new List<ResultSongDto>());

            var json = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultSongDto>>(json)
                         ?? new List<ResultSongDto>();

            return PartialView("_Recommendations", values);
        }
    }
}
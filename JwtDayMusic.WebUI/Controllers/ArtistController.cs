using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class ArtistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ArtistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> ArtistList()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Artist");

            if (!response.IsSuccessStatusCode)
            {
                // Önceden başarısız/401 yanıtları da List<ResultArtistDto> olarak
                // deserialize edilmeye çalışılıyordu; bu da null bir Model ile view'a
                // düşüp @foreach (var item in Model) üzerinde NullReferenceException'a yol açıyordu.
                return View(new List<ResultArtistDto>());
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultArtistDto>>(jsonData) ?? new List<ResultArtistDto>();
            return View(values);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Artist/Detail/{id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("ArtistList");

            var json = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<ArtistDetailDto>(json);
            return View(value);
        }
    }
}

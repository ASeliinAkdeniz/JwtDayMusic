using System.Text;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MembershipController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Paketler sayfası: mevcut kademeyi API'den çekip gösterir.
        public async Task<IActionResult> Index()
        {
            // Giriş yoksa login'e (paket almak için giriş şart).
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("SignIn", "Login");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/Membership/Current");

            string currentTier = "Basic";
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<CurrentTierDto>(json);
                currentTier = obj?.Tier ?? "Basic";
            }

            ViewBag.CurrentTier = currentTier;
            return View();
        }

        // "Satın Al" düğmesi buraya gelir; API'ye satın alma isteği atar.
        [HttpPost]
        public async Task<IActionResult> Buy(string tier)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var body = new StringContent(
                JsonConvert.SerializeObject(new { tier }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Membership/Buy", body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Response.StatusCode = 400;
                return Content(json, "application/json");
            }

            // KRİTİK: API'den dönen YENİ token'ı Session'daki eskisinin üzerine yaz.
            // Böylece kullanıcı tekrar login olmadan üst kademede olur.
            var result = JsonConvert.DeserializeObject<BuyResultDto>(json);
            if (!string.IsNullOrEmpty(result?.Token))
                HttpContext.Session.SetString("JwtToken", result.Token);

            return Content(json, "application/json");
        }
    }
}
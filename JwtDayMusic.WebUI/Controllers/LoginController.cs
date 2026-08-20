using System.Text;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JwtDayMusic.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginDto loginDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonData = JsonConvert.SerializeObject(loginDto);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("api/Login", stringContent);

            var responseJson = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                // Önceden başarısız girişte WebApi "hata" metnini token gibi 200 OK ile
                // döndürüyordu ve burada hiç kontrol edilmiyordu; kullanıcı hiçbir hata
                // görmeden geçersiz bir token ile "giriş yapmış" oluyordu.
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(loginDto);
            }

            var tokenResponse = JsonConvert.DeserializeObject<ResponseTokenDto>(responseJson);
            if (tokenResponse?.Token is null)
            {
                ModelState.AddModelError(string.Empty, "Sunucudan beklenmeyen bir yanıt alındı.");
                return View(loginDto);
            }

            HttpContext.Session.SetString("JwtToken", tokenResponse.Token);
            return RedirectToAction("Index", "Song");
        }
        public IActionResult LogOut()
        {
            // Session'daki token'ı silince kullanıcı "çıkış yapmış" olur.
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn", "Login");
        }
    }
}

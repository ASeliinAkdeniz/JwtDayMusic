using System.Text;
using System.Text.Json;
using JwtDayMusic.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtDayMusic.WebUI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterDto registerDto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("SignIn", "Login");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Kayıt başarısız: {response.StatusCode} - {responseBody}");
            return View(registerDto);
        }
    }
}

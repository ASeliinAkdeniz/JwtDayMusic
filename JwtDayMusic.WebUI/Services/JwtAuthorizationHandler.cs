using System.Net.Http.Headers;

namespace JwtDayMusic.WebUI.Services
{
    // Login sırasında Session'a yazılan JWT token'ı, WebApi'ye giden her isteğe
    // otomatik olarak "Authorization: Bearer ..." header'ı olarak ekler.
    // Bu handler olmadan WebUI, WebApi'nin [Authorize] ile korunan uçlarına
    // (ör. ArtistList) token göndermiyor ve istekler her zaman 401 dönüyordu.
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

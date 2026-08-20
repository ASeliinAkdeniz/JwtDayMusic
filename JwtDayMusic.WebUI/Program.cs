using JwtDayMusic.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Session, LoginController'ın HttpContext.Session.SetString("JwtToken", ...) çağrısı için gerekli.
// Daha önce middleware pipeline'a eklenmemişti ve bu çağrı runtime'da exception fırlatıyordu.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Session'daki JWT token'ı WebApi isteklerine otomatik ekleyen handler.
builder.Services.AddTransient<JwtAuthorizationHandler>();

// WebApi'nin adresi tek yerden (appsettings) yönetiliyor; controller'lar artık
// mutlak URL yerine bu BaseAddress'e göre relative path kullanıyor.
builder.Services.AddHttpClient("ApiClient", client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl appsettings.json içinde tanımlı değil.");
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtAuthorizationHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

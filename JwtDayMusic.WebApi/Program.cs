using System.Text;
using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Entites;
using JwtDayMusic.WebApi.Seed;
using JwtDayMusic.WebApi.Services.ArtistServices;
using JwtDayMusic.WebApi.Services.GenreServices;
using JwtDayMusic.WebApi.Services.GenreServices;
using JwtDayMusic.WebApi.Services.ImportServices;
using JwtDayMusic.WebApi.Services.LikeServices;
using JwtDayMusic.WebApi.Services.LoginServices;
using JwtDayMusic.WebApi.Services.MembershipServices;
using JwtDayMusic.WebApi.Services.MembershipServices;
using JwtDayMusic.WebApi.Services.PlaylistServices;
using JwtDayMusic.WebApi.Services.ProfileServices;
using JwtDayMusic.WebApi.Services.RecommendationServices;
using JwtDayMusic.WebApi.Services.RegisterService;
using JwtDayMusic.WebApi.Services.SongServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<JwtContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});
builder.Services.AddDbContext<JwtContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpClient();   // IHttpClientFactory için (yoksa ekle)
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedAsync(roleManager);

    var context = scope.ServiceProvider.GetRequiredService<JwtContext>();
    await DataSeeder.SeedAsync(context);
    await ListeningSeeder.SeedAsync(context);   // ← yeni
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.GenreServices
{
    public interface IGenreService
    {
        Task<List<ResultGenreDto>> GetAllGenres();
        Task<GenreDetailDto?> GetGenreDetailAsync(int id);
    }
}
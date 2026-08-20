using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.RecommendationServices
{
    public interface IRecommendationService
    {
        Task<List<ResultSongDto>> GetRecommendationsAsync(string userId, int count = 4);
    }
}
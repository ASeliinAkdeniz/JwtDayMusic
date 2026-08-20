using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.LikeServices
{
    public interface ILikeService
    {
        Task<LikeResultDto> ToggleAsync(string userId, int songId);
        Task<List<ResultSongDto>> GetMyLikesAsync(string userId);
        Task<List<int>> GetLikedSongIdsAsync(string userId);
    }
}
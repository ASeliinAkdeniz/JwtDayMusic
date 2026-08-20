using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.PlaylistServices
{
    public interface IPlaylistService
    {
        Task<ResultPlaylistDto> CreateAsync(string userId, string name);
        Task<List<ResultPlaylistDto>> GetMyPlaylistsAsync(string userId);
        Task<PlaylistDetailDto?> GetDetailAsync(string userId, int playlistId);
        Task<bool> AddSongAsync(string userId, int playlistId, int songId);
        Task<bool> RemoveSongAsync(string userId, int playlistId, int songId);
    }
}
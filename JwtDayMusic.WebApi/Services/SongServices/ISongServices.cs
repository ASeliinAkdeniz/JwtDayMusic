using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.SongServices
{
    public interface ISongService
    {
        Task<List<ResultSongDto>> GetAllSongsAsync();
        Task<PlaySongResultDto> GetPlayInfoAsync(int songId, int userTier);   // ← yeni
        Task RecordListenAsync(string userId, int songId);
        Task<List<ResultSongDto>> SearchAsync(string query);
    }
}
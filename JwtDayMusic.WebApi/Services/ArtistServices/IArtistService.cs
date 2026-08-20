using JwtDayMusic.WebApi.Dtos;

namespace JwtDayMusic.WebApi.Services.ArtistServices
{
    public interface IArtistService
    {
        Task<List<ResultArtistDto>> GetAllArtists();
        Task CreateArtistAsync(CreateArtistDto createArtistDto);
        Task<ArtistDetailDto?> GetArtistDetailAsync(int id);
    }
}

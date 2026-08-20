using JwtDayMusic.WebApi.Context;
using JwtDayMusic.WebApi.Dtos;
using JwtDayMusic.WebApi.Entites;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Services.PlaylistServices
{
    public class PlaylistService : IPlaylistService
    {
        private readonly JwtContext _context;

        public PlaylistService(JwtContext context)
        {
            _context = context;
        }

        public async Task<ResultPlaylistDto> CreateAsync(string userId, string name)
        {
            var playlist = new Playlist
            {
                Name = name,
                UserId = userId,               // sahibi = isteği yapan kullanıcı
                CreatedDate = DateTime.Now
            };
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();

            return new ResultPlaylistDto
            {
                PlaylistId = playlist.PlaylistId,
                Name = playlist.Name,
                CreatedDate = playlist.CreatedDate,
                SongCount = 0
            };
        }

        public async Task<List<ResultPlaylistDto>> GetMyPlaylistsAsync(string userId)
        {
            // SADECE bu kullanıcının playlist'leri.
            return await _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new ResultPlaylistDto
                {
                    PlaylistId = p.PlaylistId,
                    Name = p.Name,
                    CreatedDate = p.CreatedDate,
                    SongCount = p.PlaylistSongs.Count
                })
                .ToListAsync();
        }

        public async Task<PlaylistDetailDto?> GetDetailAsync(string userId, int playlistId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artist)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Genre)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            // Yok VEYA başkasına aitse → null (sahiplik kontrolü).
            if (playlist == null || playlist.UserId != userId)
                return null;

            return new PlaylistDetailDto
            {
                PlaylistId = playlist.PlaylistId,
                Name = playlist.Name,
                CreatedDate = playlist.CreatedDate,
                Songs = playlist.PlaylistSongs
                    .OrderBy(ps => ps.AddedDate)
                    .Select(ps => new ResultSongDto
                    {
                        SongId = ps.Song.SongId,
                        Title = ps.Song.Title,
                        CoverImageUrl = ps.Song.CoverImageUrl,
                        AudioUrl = ps.Song.AudioUrl,
                        Duration = ps.Song.Duration,
                        PlayCount = ps.Song.PlayCount,
                        Tier = ps.Song.Tier,
                        ReleaseDate = ps.Song.ReleaseDate,
                        ArtistId = ps.Song.ArtistId,
                        ArtistName = ps.Song.Artist.Name,
                        GenreId = ps.Song.GenreId,
                        GenreName = ps.Song.Genre.Name
                    }).ToList()
            };
        }

        public async Task<bool> AddSongAsync(string userId, int playlistId, int songId)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist == null || playlist.UserId != userId)
                return false;   // yok veya başkasının → reddet

            // Aynı şarkı zaten varsa tekrar ekleme.
            if (playlist.PlaylistSongs.Any(ps => ps.SongId == songId))
                return true;

            // Şarkı gerçekten var mı?
            bool songExists = await _context.Songs.AnyAsync(s => s.SongId == songId);
            if (!songExists) return false;

            await _context.PlaylistSongs.AddAsync(new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSongAsync(string userId, int playlistId, int songId)
        {
            var playlist = await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);

            if (playlist == null || playlist.UserId != userId)
                return false;

            var link = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (link == null) return false;

            _context.PlaylistSongs.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
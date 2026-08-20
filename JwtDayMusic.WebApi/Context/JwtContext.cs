using JwtDayMusic.WebApi.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtDayMusic.WebApi.Context
{
    public class JwtContext: IdentityDbContext<AppUser>
    {
        public JwtContext(DbContextOptions<JwtContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ListeningHistory> ListeningHistories { get; set; }
        public DbSet<Playlist> Playlists { get; set; }  
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<SongLike> SongLikes { get; set; }
    }
}

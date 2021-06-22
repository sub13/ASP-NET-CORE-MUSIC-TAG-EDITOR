using Microsoft.EntityFrameworkCore;


namespace MusicTagEditor.DataApp.Models
{
    public class SongsDbContext :DbContext
    {
        public DbSet<Song> Songs { get; set; }

        public DbSet<User> Users { get; set; }
        public SongsDbContext(DbContextOptions<SongsDbContext> options)
    : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

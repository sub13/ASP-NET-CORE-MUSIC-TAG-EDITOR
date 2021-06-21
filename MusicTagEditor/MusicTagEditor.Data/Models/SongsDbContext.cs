using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace MusicTagEditor.Data.Models
{
    public class SongsDbContext : IdentityDbContext<User>
    {
        public DbSet<Song> Songs { get; set; }

        public SongsDbContext(DbContextOptions<SongsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

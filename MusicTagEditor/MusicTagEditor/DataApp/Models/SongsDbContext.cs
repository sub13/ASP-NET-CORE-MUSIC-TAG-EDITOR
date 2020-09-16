using Microsoft.EntityFrameworkCore;
using MusicTagEditor.DataApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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

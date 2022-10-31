using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CUGOJ.CUGOJ_Admin.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(conf.GetConnectionString("DefaultConnection"));
        }
        public DbSet<Env> Envs { get; set; } = null!;
        public DbSet<UserEnv> UserEnvs { get; set; } = null!;
    }

    public class Env
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Index(nameof(UserId))]
    [Index(nameof(EnvId))]
    public class UserEnv
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public long EnvId { get; set; }
    }

}
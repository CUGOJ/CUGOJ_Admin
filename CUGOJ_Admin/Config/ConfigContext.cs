using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CUGOJ.CUGOJ_Admin.Config
{
    public class ConfigContext:DbContext
    {
        public ConfigContext()
            : base()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            options.UseSqlServer(conf.GetConnectionString("ConfigConnection"));
        }

        public DbSet<Config> Configs { get; set; } = null!;
    }

    public class Config
    {
        public int Id { get; set; }
        
        [Column(TypeName = "varchar(200)")]
        public string Env { get; set; } = "default";
        public string Value { 
            get
            {
                return JsonSerializer.Serialize(ConfigList);
            }
            set
            {
                try
                {
                    var tmp = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
                    if (tmp != null)
                        ConfigList = tmp;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        [NotMapped]
        public Dictionary<string, string> ConfigList { get; set; } = new();
                
    }

}


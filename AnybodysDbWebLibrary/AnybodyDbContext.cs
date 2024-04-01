using AnybodysWebModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace AnybodysDbWebLibrary
{
    public class AnybodyDbContext : DbContext
    {
        private static IConfigurationRoot _configuration;
        

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        //constructors

        public AnybodyDbContext()
            {
                //nothing
            }

        public AnybodyDbContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _configuration = builder.Build();
                var cnstr = _configuration.GetConnectionString("AnybodysDataDbConnection");
                optionsBuilder.UseSqlServer(cnstr);
            }
        }
    }
}

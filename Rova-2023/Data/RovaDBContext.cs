using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rova_2023.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Rova_2023.Data
{
    public class RovaDBContext : DbContext
    {
        public RovaDBContext(DbContextOptions<RovaDBContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; } = default!;

        public DbSet<CropInfo> CropInfo { get; set; } = default!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var listConverter = new ListOfStringConverter();

            modelBuilder.Entity<CropInfo>()
                .Property(e => e.Symptoms)
                .HasConversion(listConverter);

            modelBuilder.Entity<CropInfo>()
                .Property(e => e.Solutions)
                .HasConversion(listConverter);
        }
    }

    public class ListOfStringConverter : ValueConverter<List<string>, string>
    {
        public ListOfStringConverter() : base(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>())
        {
        }
    }
}

    


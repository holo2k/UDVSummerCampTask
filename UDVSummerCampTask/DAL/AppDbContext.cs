using Microsoft.EntityFrameworkCore;
using UDVSummerCampTask.DAL.Entities;

namespace UDVSummerCampTask.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<LetterFrequencyEntity> LetterFrequencies => Set<LetterFrequencyEntity>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}

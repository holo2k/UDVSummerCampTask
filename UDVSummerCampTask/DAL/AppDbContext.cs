using Microsoft.EntityFrameworkCore;
using UDVSummerCampTask.DAL.Entities;
using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<LetterFrequencyEntity> LetterFrequencies => Set<LetterFrequencyEntity>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}

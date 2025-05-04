using System.Data.Entity;
using UDVSummerCampTask.DAL.Entities;

namespace UDVSummerCampTask.DAL.Repository
{
    public class LetterFrequencyRepository : ILetterFrequencyRepository
    {
        private readonly AppDbContext context;

        public LetterFrequencyRepository(AppDbContext context)
        {
            this.context = context;  
        }

        public async Task Add(LetterFrequencyEntity entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public ICollection<LetterFrequencyEntity> GetByUserId(string userId)
        {
            var letterFrequencyList = context.LetterFrequencies.Where(x => x.UserId == userId).OrderBy(x=>x.Letter);
            return letterFrequencyList.ToList();
        }
    }
}

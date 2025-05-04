using AutoMapper;
using UDVSummerCampTask.DAL.Entities;
using UDVSummerCampTask.DAL.Repository;
using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Services.Letter
{
    public class LetterService : ILetterService
    {
        private readonly ILetterFrequencyRepository letterRepository;
        private readonly IMapper mapper;

        public LetterService(ILetterFrequencyRepository letterRepository, IMapper mapper)
        {
            this.letterRepository = letterRepository;
            this.mapper = mapper;
        }

        public async Task AddLetters(HashSet<LetterFrequency> frequencies, string userId)
        {
            foreach (var freq in frequencies)
            {
                var entity = mapper.Map<LetterFrequencyEntity>(freq);
                entity.UserId = userId;

                await letterRepository.Add(entity);
            }
        }

        public List<LetterFrequency> GetByUserId(string userId)
        {
            var freqs = letterRepository.GetByUserId(userId);

            return freqs.Select(mapper.Map<LetterFrequency>).ToList();
        }
    }
}

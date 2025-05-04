using System.Linq;
using UDVSummerCampTask.DAL.Entities;

namespace UDVSummerCampTask.DAL.Repository
{
    public interface ILetterFrequencyRepository
    {
        ICollection<LetterFrequencyEntity> GetByUserId(string userId);
        Task Add(LetterFrequencyEntity entity);
    }
}

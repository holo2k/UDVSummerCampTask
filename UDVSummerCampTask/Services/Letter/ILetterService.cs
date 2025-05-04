using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Services.Letter
{
    public interface ILetterService
    {
        Task AddLetters(HashSet<LetterFrequency> frequencies, string userId);
        List<LetterFrequency> GetByUserId(string userId);
    }
}

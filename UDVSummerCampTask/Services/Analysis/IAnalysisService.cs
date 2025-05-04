using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Services.Analysis
{
    public interface IAnalysisService
    {
        public HashSet<LetterFrequency> CountLetters(IEnumerable<string> texts);
    }
}

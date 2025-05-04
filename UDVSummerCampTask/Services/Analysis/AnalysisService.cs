using UDVSummerCampTask.Models;

namespace UDVSummerCampTask.Services.Analysis
{
    public class AnalysisService : IAnalysisService
    {
        public HashSet<LetterFrequency> CountLetters(IEnumerable<string> texts)
        {
            return texts
                .SelectMany(t => t.ToLowerInvariant())
                .Where(char.IsLetter)
                .GroupBy(c => c)
                .Select(g => new LetterFrequency
                {
                    Letter = g.Key,
                    Count = g.Count(),
                    CalculatedAt = DateTime.UtcNow
                })
                .ToHashSet();
        }
    }
}

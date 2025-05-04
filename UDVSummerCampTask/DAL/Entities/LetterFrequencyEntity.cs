namespace UDVSummerCampTask.DAL.Entities
{
    public class LetterFrequencyEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public char Letter { get; set; }
        public int Count { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}

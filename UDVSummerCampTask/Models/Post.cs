namespace UDVSummerCampTask.Models
{
    public class Post
    {
        public int UserId { get; set; }
        public string Content { get; set; }

        public Post(int UserId, string Content)
        {
            this.UserId = UserId;
            this.Content = Content;
        }
    }
}

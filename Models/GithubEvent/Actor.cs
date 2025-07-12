namespace GitHubActivity
{
    public class Actor : BaseEntity
    {
        public string login { get; set; }
        public string gravataId { get; set; }
        public string url { get; set; }
        public string avatarUrl { get; set; }
    }
}
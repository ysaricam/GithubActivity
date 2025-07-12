namespace GitHubActivity
{

    public class GithubEvent : BaseEntity
    {
        public string type { get; set; }
        public Actor actor { get; set; }
        public Repository repo { get; set; }
        public Payload payload { get; set; }
        public string publicValue { get; set; }
        public DateTime createdAt { get; set; }
        public Org org { get; set; }
    }
}
namespace GitHubActivity
{

    public class GithubEvent
    {
        public string id { get; set; }
        public string type { get; set; }
        public Actor actor { get; set; }
        public Repository repo { get; set; }
        public Payload payload { get; set; }
        public bool @public { get; set; }
        public DateTime createdAt { get; set; }
        public Org org { get; set; }
    }
}
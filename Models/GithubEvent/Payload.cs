namespace GitHubActivity
{
    public class Payload
    {
        public string action { get; set; }
        public int number { get; set; }
        public PullRequest pull_request { get; set; }
        public Issue issue { get; set; }
        public List<Commit> commits { get; set; }
        public string ref_value { get; set; }
        public string ref_type { get; set; }
        public string master_branch { get; set; }
        public string description { get; set; }
        public int size { get; set; }
        public int distinct_size { get; set; }
        public string head { get; set; }
        public string before { get; set; }
        public List<string> pages { get; set; }
    }
}
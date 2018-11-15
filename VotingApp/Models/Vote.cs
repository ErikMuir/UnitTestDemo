namespace VotingApp.Models
{
    public class Vote
    {
        public int CitizenId { get; set; }
        public int BallotItemId { get; set; }
        public int BallotItemOption { get; set; }
        public string WriteIn { get; set; } = null;
    }
}

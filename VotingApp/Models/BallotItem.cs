using System.Collections.Generic;

namespace VotingApp.Models
{
    public class BallotItem
    {
        public int BallotItemId { get; set; }
        public List<BallotItemOption> Options { get; set; }
        public bool IsWriteInOptionAvailable { get; set; }
    }
}

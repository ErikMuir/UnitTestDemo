using System.Linq;
using VotingApp.Exceptions;
using VotingApp.Models;
using VotingApp.Repositories;

namespace VotingApp.Services
{
    public class VotingService
    {
        private readonly IVotingRepository _repo;

        public VotingService(IVotingRepository repo)
        {
            _repo = repo;
        }

        public VoteConfirmation CastVote(int citizenId, int ballotItemId, int ballotItemOption, string writeIn = null)
        {
            // Verify that the citizen exists
            var citizen = _repo.GetCitizen(citizenId);
            if (citizen == null)
                throw new RecordNotFoundException("Could not find that citizen");

            // Verify that the ballot item exists
            var ballotItem = _repo.GetBallotItem(ballotItemId);
            if (ballotItem == null)
                throw new RecordNotFoundException("Could not find that ballot item");

            // Verify that the citizen is eligible to cast a vote on the ballot item
            var isEligible = _repo.IsCitizenEligibleToVoteOnBallotItem(citizenId, ballotItemId);
            if (!isEligible)
                throw new IneligibleVoteException("That citizen is not eligible to vote on that ballot item");

            // Verify that the citizen hasn't already cast a vote on the ballot item
            var vote = _repo.GetVote(citizenId, ballotItemId);
            if (vote != null)
                throw new AlreadyVotedException("That citizen has already voted on that ballot item");

            // Verify that the chosen option is valid
            if (ballotItemOption == 0)
            {
                if (!ballotItem.IsWriteInOptionAvailable)
                    throw new InvalidVoteException("The write-in option is not available for that ballot item");
                if (string.IsNullOrWhiteSpace(writeIn))
                    throw new InvalidVoteException("The write-in value cannot be null or empty");
            }
            else if (ballotItem.Options.All(option => option.BallotItemOptionId != ballotItemOption))
            {
                throw new InvalidVoteException("That option is not valid");
            }

            // Cast the vote
            var voteConfirmation = _repo.AddVote(citizenId, ballotItemId, ballotItemOption, writeIn);
            return voteConfirmation;
        }
    }
}

using VotingApp.Models;

namespace VotingApp.Repositories
{
    public interface IVotingRepository
    {
        Citizen GetCitizen(int citizenId);

        BallotItem GetBallotItem(int ballotItemId);

        bool IsCitizenEligibleToVoteOnBallotItem(int citizenId, int ballotItemId);

        Vote GetVote(int citizenId, int ballotItemId);

        VoteConfirmation AddVote(int citizenId, int ballotItemId, int ballotItemOption, string writeIn);
    }
}

using FakeItEasy;
using System.Collections.Generic;
using VotingApp.Exceptions;
using VotingApp.Models;
using VotingApp.Repositories;
using VotingApp.Services;
using Xunit;

namespace VotingApp.UnitTests
{
    public class CastVoteTests
    {
        #region -- Constructor --

        public CastVoteTests()
        {
            _repo = A.Fake<IVotingRepository>(options => options.Strict());
            A.CallTo(() => _repo.GetCitizen(CitizenId)).Returns(Citizen);
            A.CallTo(() => _repo.GetBallotItem(BallotItemId)).Returns(BallotItem);
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(CitizenId, BallotItemId)).Returns(true);
            A.CallTo(() => _repo.GetVote(CitizenId, BallotItemId)).Returns(null);
            A.CallTo(() => _repo.AddVote(CitizenId, BallotItemId, A<int>._, A<string>._)).Returns(VoteConfirmation);
            _service = A.Fake<VotingService>(options => options
                .Strict()
                .WithArgumentsForConstructor(new object[] { _repo }));
            A.CallTo(() => _service.CastVote(A<int>._, A<int>._, A<int>._, A<string>._))
                .CallsBaseMethod();
        }

        #endregion

        #region -- Private Members/Properties --

        private IVotingRepository _repo { get; set; }
        private VotingService _service { get; set; }

        private const int CitizenId = 12345;
        private const int BallotItemId = 123;
        private const int BallotItemOption = 1;
        private const int WriteInOption = 0;
        private const string WriteInValue = "Chris Kuroda";
        private static readonly Citizen Citizen = new Citizen { CitizenId = CitizenId };
        private static readonly BallotItemOption TreyAnastasio = new BallotItemOption
        {
            BallotItemOptionId = 1,
            Name = "Trey Anastasio",
            Description = "Guitarist and lead vocalist for the band Phish",
        };
        private static readonly BallotItemOption PageMcConnell = new BallotItemOption
        {
            BallotItemOptionId = 2,
            Name = "Page McConnell",
            Description = "Keyboardist and vocalist for the band Phish",
        };
        private static readonly BallotItemOption MikeGordon = new BallotItemOption
        {
            BallotItemOptionId = 3,
            Name = "Mike Gordon",
            Description = "Bassist and vocalist for the band Phish",
        };
        private static readonly BallotItemOption JonFishman = new BallotItemOption
        {
            BallotItemOptionId = 4,
            Name = "Jon Fishman",
            Description = "Drummer and vocalist for the band Phish",
        };
        private static readonly List<BallotItemOption> Options = new List<BallotItemOption>
        {
            TreyAnastasio,
            PageMcConnell,
            MikeGordon,
            JonFishman,
        };
        private static readonly BallotItem BallotItem = new BallotItem
        {
            BallotItemId = BallotItemId,
            Options = Options,
            IsWriteInOptionAvailable = false,
        };
        private static readonly BallotItem BallotItemWithWriteIn = new BallotItem
        {
            BallotItemId = BallotItemId,
            Options = Options,
            IsWriteInOptionAvailable = true,
        };
        private static readonly Vote Vote = new Vote
        {
            CitizenId = CitizenId,
            BallotItemId = BallotItemId,
            BallotItemOption = BallotItemOption,
            WriteIn = null,
        };
        private static readonly VoteConfirmation VoteConfirmation = new VoteConfirmation
        {
            Vote = Vote,
            Success = true,
        };

        #endregion

        [Fact]
        public void CastVote_NoCitizen_Throws()
        {
            // Arrange
            A.CallTo(() => _repo.GetCitizen(CitizenId)).Returns(null);

            // Act 
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, BallotItemOption, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<RecordNotFoundException>(exception);
            Assert.Equal("Could not find that citizen", exception.Message);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_NoBallotItem_Throws()
        {
            // Arrange
            A.CallTo(() => _repo.GetBallotItem(BallotItemId)).Returns(null);

            // Act
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, BallotItemOption, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<RecordNotFoundException>(exception);
            Assert.Equal("Could not find that ballot item", exception.Message);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_NotEligible_Throws()
        {
            // Arrange
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(CitizenId, BallotItemId)).Returns(false);

            // Act
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, BallotItemOption, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<IneligibleVoteException>(exception);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_VoteExists_Throws()
        {
            // Arrange
            A.CallTo(() => _repo.GetVote(CitizenId, BallotItemId)).Returns(Vote);

            // Act
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, BallotItemOption, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<AlreadyVotedException>(exception);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_WriteInNotAllowed_Throws()
        {
            // Act
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, WriteInOption, WriteInValue));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidVoteException>(exception);
            Assert.Equal("The write-in option is not available for that ballot item", exception.Message);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_WriteInNotProvided_Throws()
        {
            // Arrange
            A.CallTo(() => _repo.GetBallotItem(BallotItemId)).Returns(BallotItemWithWriteIn);

            // Act
            var exception = Record.Exception(() => _service.CastVote(CitizenId, BallotItemId, WriteInOption, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidVoteException>(exception);
            Assert.Equal("The write-in value cannot be null or empty", exception.Message);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void CastVote_Works()
        {
            // Act
            var actualResult = _service.CastVote(CitizenId, BallotItemId, BallotItemOption, null);

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<VoteConfirmation>(actualResult);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CastVote_WriteIn_Works()
        {
            // Arrange 
            A.CallTo(() => _repo.GetBallotItem(BallotItemId)).Returns(BallotItemWithWriteIn);

            // Act
            var actualResult = _service.CastVote(CitizenId, BallotItemId, WriteInOption, WriteInValue);

            // Assert
            Assert.NotNull(actualResult);
            Assert.IsType<VoteConfirmation>(actualResult);
            A.CallTo(() => _repo.GetCitizen(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetBallotItem(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.IsCitizenEligibleToVoteOnBallotItem(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.GetVote(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.AddVote(A<int>._, A<int>._, A<int>._, A<string>._)).MustHaveHappenedOnceExactly();
        }
    }
}

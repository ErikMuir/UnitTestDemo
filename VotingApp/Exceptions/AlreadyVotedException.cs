using System;

namespace VotingApp.Exceptions
{
    public class AlreadyVotedException : Exception
    {
        public AlreadyVotedException() : base() { }
        public AlreadyVotedException(string message) : base(message) { }
        public AlreadyVotedException(string message, Exception innerException) : base(message, innerException) { }
    }
}

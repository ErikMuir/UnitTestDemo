using System;

namespace VotingApp.Exceptions
{
    public class InvalidVoteException : Exception
    {
        public InvalidVoteException() : base() { }
        public InvalidVoteException(string message) : base(message) { }
        public InvalidVoteException(string message, Exception innerException) : base(message, innerException) { }
    }
}

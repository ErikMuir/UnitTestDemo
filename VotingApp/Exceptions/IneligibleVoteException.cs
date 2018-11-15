using System;

namespace VotingApp.Exceptions
{
    public class IneligibleVoteException : Exception
    {
        public IneligibleVoteException() : base() { }
        public IneligibleVoteException(string message) : base(message) { }
        public IneligibleVoteException(string message, Exception innerException) : base(message, innerException) { }
    }
}

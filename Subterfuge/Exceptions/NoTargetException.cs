using System;

namespace Subterfuge.Exceptions
{
    /// <summary>
    /// An exception to be thrown when an agent tries to act without a target.
    /// </summary>
    public class NoTargetException : Exception
    {
        public Type AgentType { get; protected set; }

        public NoTargetException(Type agentType)
        {
            AgentType = agentType;
        }

        public NoTargetException(Type agentType, string message)
            : base(message)
        {
            AgentType = agentType;
        }

        public NoTargetException(Type agentType, string message, Exception inner)
            : base(message, inner)
        {
            AgentType = agentType;
        }
    }
}

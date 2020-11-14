using Subterfuge.Enums;
using System;

namespace Subterfuge.Exceptions
{
    public class NoTargetException : Exception
    {
        public AgentType AgentType { get; protected set; }

        public NoTargetException(AgentType agentType)
        {
            AgentType = agentType;
        }

        public NoTargetException(AgentType agentType, string message)
            : base(message)
        {
            AgentType = agentType;
        }

        public NoTargetException(AgentType agentType, string message, Exception inner)
            : base(message, inner)
        {
            AgentType = agentType;
        }
    }
}

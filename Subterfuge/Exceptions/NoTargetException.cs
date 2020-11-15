using Subterfuge.Enums;
using System;

namespace Subterfuge.Exceptions
{
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

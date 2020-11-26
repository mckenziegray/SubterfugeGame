using System;
using System.Collections.Generic;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class AgentList
    {
        // NOTE: This key must be the name of the class without any namespace. This can be obtained using nameof(AgentClass) or AgentInstance.GetType().Name
        public Agent this[string key] => Dictionary[key];
        public int Count => Dictionary.Count;
        public List<Agent> OrderedList { get; protected set; }
        public List<Agent> ShuffledList { get; protected set; }
        public List<PlayerAgent> PlayerAgents { get; protected set; }

        protected Dictionary<string, Agent> Dictionary { get; set; }

        /// <summary>
        /// Initializes the list and populates it with one agent of each type.
        /// </summary>
        public AgentList()
        {
            OrderedList = new();
            ShuffledList = new();
            Dictionary = new();
            PlayerAgents = new();

            foreach (Type type in GameService.AGENT_TYPES_ORDERED)
            {
                Agent agent = type.Instantiate<Agent>();

                OrderedList.Add(agent);
                ShuffledList.Add(agent);
                Dictionary.Add(type.Name, agent);

                if (agent is PlayerAgent playerAgent)
                    PlayerAgents.Add(playerAgent);
            }

            ShuffledList.Shuffle(GameService.Random);
        }
    }
}

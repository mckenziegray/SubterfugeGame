using System;
using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class AgentList
    {
        public Agent this[AgentType key] => Dictionary[key];

        public List<Agent> OrderedList { get; protected set; }
        public List<Agent> ShuffledList { get; protected set; }
        public List<Agent> Allies { get; protected set; }

        public int Count => Dictionary.Count;

        protected Dictionary<AgentType, Agent> Dictionary { get; set; }

        public AgentList()
        {
            OrderedList = new();
            ShuffledList = new();
            Dictionary = new();

            foreach (AgentType agentType in Enum.GetValues(typeof(AgentType)).Cast<AgentType>())
            {
                Agent agent = new Agent(agentType);

                Dictionary.Add(agentType, agent);
                OrderedList.Add(agent);
            }

            ShuffledList = new List<Agent>(OrderedList);
            ShuffledList.Shuffle(GameService.Random);

            Allies = OrderedList.Where(a => a.Allegiance == Allegiance.Ally).ToList();
        }
    }
}

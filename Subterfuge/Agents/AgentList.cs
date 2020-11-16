using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class AgentList
    {
        // NOTE: This key must be the name of the class without any namespace. This can be obtained using nameof(AgentClass) or AgentInstance.GetType().Name
        public Agent this[string key] => Dictionary[key];

        public List<Agent> OrderedList { get; protected set; }
        public List<Agent> ShuffledList { get; protected set; }
        public List<Agent> Allies { get; protected set; }

        public int Count => Dictionary.Count;

        protected Dictionary<string, Agent> Dictionary { get; set; }

        public AgentList()
        {
            ShuffledList = new();
            Dictionary = new();

            // NOTE: This is the order in which agents will act
            OrderedList = new List<Agent>
            {
                new Saboteur(),
                new Swallow(),
                new Marshal(),
                new Convoy(),
                new Medic(),
                new Android(),
                new Assassin(),
                new Drudge(),
                new Mastermind(),
                new Fabricator(),
                new Cutout(),
                new Interrogator(),
                new Hacker(),
                new Sentry(),
                new Sleeper()
            };

            foreach (Agent agent in OrderedList)
            {
                Dictionary.Add(agent.GetType().Name, agent);
            }

            ShuffledList = new List<Agent>(OrderedList);
            ShuffledList.Shuffle(GameService.Random);

            Allies = OrderedList.Where(a => a.Allegiance == Allegiance.Ally).ToList();
        }
    }
}

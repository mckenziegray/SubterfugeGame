using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class AgentList
    {
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

            OrderedList = new List<Agent>
            {
                new Marshal(),
                new Swallow(),
                new Saboteur(),
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
                Dictionary.Add(agent.GetType().ToString(), agent);
            }

            ShuffledList = new List<Agent>(OrderedList);
            ShuffledList.Shuffle(GameService.Random);

            Allies = OrderedList.Where(a => a.Allegiance == Allegiance.Ally).ToList();
        }
    }
}

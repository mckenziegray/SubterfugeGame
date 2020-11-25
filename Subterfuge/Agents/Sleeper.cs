using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Sleeper : NonPlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;

        public Sleeper() : base() { }

        protected override void Act()
        {
            // Does nothing
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.Allegiance == Allegiance.Ally && a.IsAlive).ToList();
            if (validTargets.Any())
                Target = validTargets[GameService.Random.Next(validTargets.Count)];

            // The sleeper is special - they pick a target but do nothing unless executed
            IsActing = false;
        }
    }
}

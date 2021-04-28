using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Cutout : NonPlayerAgent
    {
        public override string Name => "Cut-out";
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;

        public Cutout() : base() { }

        protected override void Act()
        {
            if (Target != this && VisitTarget)
                Target.Visit(this);
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.IsActive).ToList();

            if (validTargets.Any())
            {
                Target = validTargets[GameService.Random.Next(validTargets.Count)];
                IsActing = true;
            }
        }
    }
}

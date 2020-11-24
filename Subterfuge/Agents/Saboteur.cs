using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Saboteur : NonPlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Saboteur() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Block(this);
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.Allegiance == Allegiance.Ally && a.IsAlive).ToList();

            if (validTargets.Any())
            {
                Target = validTargets[GameService.Random.Next(validTargets.Count)];
                IsActing = true;
            }
        }
    }
}

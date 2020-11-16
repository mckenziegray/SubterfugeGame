using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Saboteur : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Saboteur() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Block(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.Allegiance != Allegiance.Enemy && a.IsAlive).ToList();

            if (validTargets.Any())
            {
                Target = validTargets[GameService.Random.Next(validTargets.Count)];
                IsActing = true;
            }
        }
    }
}

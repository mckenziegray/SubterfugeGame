using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Fabricator : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        private const double CHANCE_TO_TARGET_SLEEPER = 0.5;

        public Fabricator() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Frame(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => 
                a != this && 
                a.Allegiance != Allegiance.Enemy && 
                a.IsAlive &&
                a != agents[nameof(Mastermind)].Target &&
                a != agents[nameof(Drudge)].Target &&
                a is not Sleeper // Sleeper is a valid target but it's treated differently
            ).ToList();

            // Give the Sleeper a higher chance of being selected than the others
            if (agents[nameof(Sleeper)].IsAlive && GameService.Random.NextDouble() <= CHANCE_TO_TARGET_SLEEPER)
            {
                Target = agents[nameof(Sleeper)];
                IsActing = true;

            }
            else if (validTargets.Any())
            {
                Target = validTargets[GameService.Random.Next(validTargets.Count)];
                IsActing = true;
            }
        }
    }
}

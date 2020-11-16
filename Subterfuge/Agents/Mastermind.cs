using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Mastermind : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        private const double CHANCE_TO_ATTACK = 0.9;

        public Mastermind() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Attack(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }

        public override void SelectTarget(AgentList agents)
        {
            if (!agents[nameof(Drudge)].IsAlive && GameService.Random.NextDouble() <= CHANCE_TO_ATTACK)
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
}

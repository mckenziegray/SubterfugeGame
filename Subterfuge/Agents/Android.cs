using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Android : NonPlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        /// <summary>
        /// The chance that this agent will try to act on any given round.
        /// </summary>
        private const double CHANCE_TO_ATTACK = 0.95;

        public Android() : base() { }

        public override void SelectTarget(AgentList agents)
        {
            if (GameService.Random.NextDouble() <= CHANCE_TO_ATTACK)
            {
                List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.IsActive && a is not Mastermind).ToList();

                if (validTargets.Any())
                {
                    Target = validTargets[GameService.Random.Next(validTargets.Count)];
                    IsActing = true;
                }
            }
        }

        protected override void Act()
        {
            if (Target.IsActive && Target != this)
                Target.Attack(this);
        }
    }
}

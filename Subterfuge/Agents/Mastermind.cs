using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Mastermind : NonPlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        /// <summary>
        /// The chance that this agent will try to act on any given round.
        /// </summary>
        private const double CHANCE_TO_ATTACK = 0.9;

        public Mastermind() : base() { }

        protected override void Act()
        {
            if (Target.IsActive && Target != this)
                Target.Attack(this);
        }

        public override void SelectTarget(AgentList agents)
        {
            Agent drudge = agents[nameof(Drudge)];
            if (!drudge.IsActive && GameService.Random.NextDouble() <= CHANCE_TO_ATTACK)
            {
                if (drudge.Target?.IsActive == true)
                {
                    Target = drudge.Target;
                    IsActing = true;
                }
                else
                {
                    List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.Allegiance == Allegiance.Ally && a.IsActive).ToList();

                    if (validTargets.Any())
                    {
                        Target = validTargets[GameService.Random.Next(validTargets.Count)];
                        IsActing = true;
                    }
                }
            }
        }
    }
}

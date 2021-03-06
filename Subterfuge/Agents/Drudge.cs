﻿using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Drudge : NonPlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Drudge() : base() { }

        protected override void Act()
        {
            if (Target.IsActive && Target != this)
                Target.Attack(this);
        }

        public override void SelectTarget(AgentList agents)
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

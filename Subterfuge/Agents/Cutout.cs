﻿using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Cutout : Agent
    {
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;
        public override string Name => "Cut-out";

        public Cutout() : base() { }

        protected override void Act()
        {
            if (Target != this)
                Target.Visit(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }

        public override void SelectTarget(AgentList agents)
        {
            List<Agent> validTargets = agents.ShuffledList.Where(a => a != this && a.IsAlive).ToList();

            if (validTargets.Any())
            {
                Target = validTargets[GameService.Random.Next(validTargets.Count)];
                IsActing = true;
            }
        }
    }
}

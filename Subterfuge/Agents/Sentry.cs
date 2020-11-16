using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Sentry : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Sentry() : base() { }

        protected override void Act()
        {
            // Does nothing
        }

        public override string GetReport()
        {
            string report = $"I got your request to keep an eye on {Target.Codename}'s quarters for the night";
            if (IsBlocked)
                report += $" but something else came up. Sorry.";
            else if (Target.Visitors.Count == 0)
                report += $". {Target.Gender.ToCommonPronoun()} had no visitors.";
            else
                report += $". {Target.Gender.ToCommonPronoun()} had {Target.Visitors.Count} {(Target.Visitors.Count == 1 ? "visitor" : "visitors")}: {string.Join(", ", Target.Visitors)}";
            
            return report;
        }

        public override void SelectTarget(AgentList agents)
        {
            throw new NotSupportedException();
        }
    }
}

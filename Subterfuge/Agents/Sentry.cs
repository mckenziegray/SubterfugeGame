using System;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Sentry : PlayerAgent
    {
        public override bool RequiresTarget => true;

        public Sentry() : base() { }

        protected override void Act()
        {
            // Does nothing
        }

        public override string GetReport()
        {
            return $"I got your request to keep an eye on {Target.Codename}'s quarters for the night" + GetReportType() switch
            {
                ReportType.Action => $". {Target.Gender.ToCommonPronoun()} had" +
                    (Target.Visitors.Count == 0
                    ? " no visitors."
                    : $" {Target.Visitors.Count} {(Target.Visitors.Count == 1 ? "visitor" : "visitors")}: {string.Join(", ", Target.Visitors)}."),
                ReportType.Blocked => $" but something else came up. Sorry.",
                _ => throw new NotImplementedException()
            };
        }

        protected override string GetReportBriefAction()
        {
            return $" {Target.Codename} was visited by" + Target.Visitors.Count switch
            {
                0 => " no one.",
                1 => $" {Target.Visitors.Single()}.",
                _ => $": {string.Join(", ", Target.Visitors)}."
            };
        }

        protected override ReportType GetReportType()
        {
            if (IsBlocked)
                return ReportType.Blocked;
            else
                return ReportType.Action;
        }
    }
}

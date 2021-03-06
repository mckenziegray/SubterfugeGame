﻿using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Convoy : PlayerAgent
    {
        public override bool RequiresTarget => true;

        public Convoy() : base() { }

        protected override void Act()
        {
            if (Target.IsActive)
                Target.Protect(this, true);
        }

        public override string GetReport()
        {
            return $"I received your orders to protect {Target.Codename}."
                + GetReportType() switch
                {
                    ReportType.Action => $" The night was uneventful, and {Target.Gender.ToCommonPronoun().ToLower()} was not harmed.",
                    ReportType.Blocked => " However, I was preocupied. My apologies.",
                    _ => throw new NotImplementedException()
                };
        }

        protected override string GetReportBriefAction()
        {
            return $" {Target.Codename} was visited by: {string.Join(", ", Target.Visitors)}";
        }

        protected override ReportType GetReportType()
        {
            if (Target == this || IsBlocked)
                return ReportType.Blocked;
            else
                return ReportType.Action;
        }
    }
}

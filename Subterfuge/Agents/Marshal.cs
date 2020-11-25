using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Marshal : PlayerAgent
    {
        public override bool RequiresTarget => true;

        public Marshal() : base() { }

        protected override void Act()
        {
            if (Target is Android || (Target is Mastermind && Target.IsActing))
            {
                // The Android is immune to being role-blocked and will instead try to kill the unit targeting it
                // The Mastermind will do the same only if the Drudge is dead
                Target.Visit(this);
                Target.Target = this;
                Target.IsActing = true;
            }
            else if (Target.IsAlive && Target != this)
            {
                Target.Block(this);
                Target.Protect(this);
            }
        }

        public override string GetReport()
        {
            return GetReportType() switch
            {
                ReportType.Action => $"As requested, {Target.Codename} was detained all night.",
                ReportType.Blocked => $"Regrettably, I was unable to hold {Target.Codename} overnight.",
                ReportType.SelfIdentify => $"Sorry, boss; {Target.Codename} is me. I could lock myself up, but it wouldn't do much good since I have the key.",
                _ => throw new NotImplementedException()
            };
        }

        public override ReportType GetReportType()
        {
            if (Target == this)
                return ReportType.SelfIdentify;
            else if (Target.Blocker == this)
                return ReportType.Action;
            else
                return ReportType.Blocked;
        }
    }
}

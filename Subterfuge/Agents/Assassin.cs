using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Assassin : PlayerAgent
    {
        public override bool RequiresTarget => true;

        public Assassin() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this && Target is not Mastermind or Android or Sleeper)
                Target.Attack(this);
        }

        public override string GetReport()
        {
            return GetReportType() switch
            {
                ReportType.Action => $"Orders received. Target neutralized.",
                ReportType.Blocked => $"Mission compromised. Standing by for further orders.",
                _ => throw new NotImplementedException()
            };
        }

        public override ReportType GetReportType()
        {
            return Target.Killer == this ? ReportType.Action : ReportType.Blocked;
        }
    }
}

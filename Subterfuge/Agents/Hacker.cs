using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Hacker : PlayerAgent
    {
        protected enum HackerReportType
        {
            Android,
            Enemy,
            NonEnemy,
            Blocked,
            SelfIdentify
        }

        public override bool RequiresTarget => true;

        public Hacker() : base() { }

        protected override void Act()
        {
            // Does nothing
        }

        public override string GetReport()
        {
            HackerReportType reportType = GetHackerReportType();
            return "I got your dead drop" + reportType switch
            {
                HackerReportType.Blocked => ", boss. Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.",
                _ => $" and I did some digging into {Target.Codename}." + reportType switch
                {
                    HackerReportType.Android => $" After navigating through layers of encryption, I found nothing but learning models and machine code. No doubt, it's the {Target.Name}.",
                    HackerReportType.Enemy => $" On their hard drive, I found strategems, coded communiqués, agent dossiers... really shady stuff. Make of that what you will.",
                    HackerReportType.NonEnemy => $" They seem clean. I wasn't able to find anything suspicious.",
                    _ => throw new NotImplementedException()
                }
            };
        }

        protected override string GetReportBriefAction()
        {
            return $" {Target.Codename}" + GetHackerReportType() switch
            {
                HackerReportType.Android    => $" is the Android.",
                HackerReportType.Enemy      => $" appears suspiscious.",
                HackerReportType.NonEnemy   => $" appears innocent.",
                _ => throw new NotImplementedException()
            };
        }

        protected override ReportType GetReportType()
        {
            return GetHackerReportType() switch
            {
                HackerReportType.Android or
                HackerReportType.Enemy or
                HackerReportType.NonEnemy => ReportType.Action,
                HackerReportType.Blocked => ReportType.Blocked,
                _ => throw new NotImplementedException()
            };
        }

        protected HackerReportType GetHackerReportType()
        {
            if (!Target.IsActive || IsBlocked)
                return HackerReportType.Blocked;
            else if(Target is Android)
                return HackerReportType.Android;
            else if (Target.WasFramed || (Target.Allegiance == Allegiance.Enemy && Target is not Mastermind))
                return HackerReportType.Enemy;
            else
                return HackerReportType.NonEnemy;
        }
    }
}

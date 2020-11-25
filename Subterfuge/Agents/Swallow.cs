using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Swallow : PlayerAgent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;
        public override string Name => Gender switch
        {
            Gender.Male => "Raven",
            Gender.Female or _ => "Swallow"
        };

        public Swallow() : base() { }

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
            if (Target is null)
                throw new ArgumentNullException(nameof(Target));

            return $"I received your orders to carry out the \"special\" operation with {Target.Codename}."
                + GetReportType() switch
                {
                    ReportType.Action => $" The mission was a success. {Target.Gender.ToCommonPronoun()} was tied up all night.",
                    ReportType.Blocked => " I'm sorry to say that I was unsuccessful.",
                    ReportType.SelfIdentify => " Actually, that's me. But don't worry, I kept myself entertained.",
                    _ => throw new NotImplementedException()
                };
        }

        public override ReportType GetReportType()
        {
            if (Target is null)
                throw new ArgumentNullException(nameof(Target));

            if (Target == this)
                return ReportType.SelfIdentify;
            else if (Target.Blocker == this)
                return ReportType.Action;
            else
                return ReportType.Blocked;
        }
    }
}

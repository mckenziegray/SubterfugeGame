using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Swallow : Agent
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
            string report = $"I received your orders to carry out the \"special\" operation with {Target.Codename}.";
            if (Target == this)
                report += " Actually, that's me. But don't worry, I kept myself entertained.";
            else if (Target.Blocker == this)
                report += $" The mission was a success. {Target.Gender.ToCommonPronoun()} was tied up all night.";
            else
                report += " I'm sorry to say that I was unsuccessful.";

            return report;
        }

        public override void SelectTarget(AgentList agents)
        {
            throw new NotSupportedException();
        }
    }
}

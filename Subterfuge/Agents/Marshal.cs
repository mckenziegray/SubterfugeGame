using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Marshal : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Marshal() : base() { }

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
            string report;
            if (Target == this)
                report = $"Sorry, boss; {Target.Codename} is me. I could lock myself up, but it wouldn't do much good since I have the key.";
            else if (Target.Blocker == this)
                report = $"As requested, {Target.Codename} was detained all night.";
            else
                report = $"Regrettably, I was unable to hold {Target.Codename} overnight.";

            return report;
        }
    }
}

using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Hacker : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Hacker() : base() { }

        protected override void Act()
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string GetReport()
        {
            string report = "I got your dead drop, boss.";
            if (!Target.IsAlive || IsBlocked)
            {
                report += " Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.";
            }
            else
            {
                report += $" I did some digging into {Target.Codename}.";
                // TODO: Get information
            }

            return report;
        }
    }
}

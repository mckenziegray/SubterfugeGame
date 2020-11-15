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
            // Does nothing
        }

        public override string GetReport()
        {
            string report = "I got your dead drop";
            if (!Target.IsAlive || IsBlocked)
            {
                report += ", boss. Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.";
            }
            else
            {
                report += $" and I did some digging into {Target.Codename}.";

                /*
                 * Scenarios:
                 * 1. Guaranteed identification (this will always happen for Android)
                 * 2. Looks like a bad guy (this will always happen for the Fabricator's target)
                 * 3. Looks like a good guy (this will always happen for Mastermind)
                 */

                if (Target is Android)
                    report += $" After navigating through layers of encryption, I found nothing but learning models and machine code. No doubt, it's the {Target.Name}.";
                else if (Target.WasFramed || (Target.Allegiance == Allegiance.Enemy && Target is not Mastermind))
                    report += $" On their hard drive, I found strategems, coded communiqués, agent dossiers... really shady stuff. Make of that what you will.";
                else
                    report += $" They seem clean. I wasn't able to find anything suspicious.";
            }

            return report;
        }
    }
}

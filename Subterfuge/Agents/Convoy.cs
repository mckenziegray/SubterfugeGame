using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Convoy : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Convoy() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive)
                Target.Protect(this, true);
        }

        public override string GetReport()
        {
            string report = $"I received your orders to protect {Target.Codename}.";
            if (IsBlocked)
                report += " However, I was preocupied. My apologies.";
            else
                report += $" The night was uneventful, and {Target.Gender.ToCommonPronoun().ToLower()} was not harmed.";

            return report;
        }
    }
}

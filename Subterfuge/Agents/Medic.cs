using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Medic : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Medic() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Protect(this, false);
        }

        public override string GetReport()
        {
            string report = $"I got your orders to look after {Target.Codename}.";
            if (!Target.IsAlive || IsBlocked)
                report += " Unfortunately, I was unable to perform my duties. Sorry.";
            else if (Target.WasAttacked)
                report += $" {Target.Gender.ToCommonPronoun()} was attacked but is alive and recovering.";
            else
                report += " Thankfully, my services were not required.";

            return report;
        }

        public override void SelectTarget(AgentList agents)
        {
            throw new NotSupportedException();
        }
    }
}

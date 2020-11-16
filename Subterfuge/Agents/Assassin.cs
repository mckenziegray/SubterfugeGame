using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Assassin : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Assassin() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Attack(this);
        }

        public override string GetReport()
        {
            string report;
            if (Target.Killer == this)
                report = $"Orders received. Target neutralized.";
            else
                report = $"Mission compromised. Standing by for further orders.";

            return report;
        }

        public override void SelectTarget(AgentList agents)
        {
            throw new NotSupportedException();
        }
    }
}

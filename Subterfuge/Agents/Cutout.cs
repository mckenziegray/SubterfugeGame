using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Cutout : Agent
    {
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;

        public Cutout() : base() { }

        protected override void Act()
        {
            if (Target != this)
                Target.Visit(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }
    }
}

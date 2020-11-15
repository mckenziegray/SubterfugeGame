using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Sleeper : Agent
    {
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;

        public Sleeper() : base() { }

        protected override void Act()
        {
            // Does nothing
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }
    }
}

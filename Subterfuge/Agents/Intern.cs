using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Intern : Agent
    {
        public override Allegiance Allegiance => Allegiance.Neutral;
        public override bool RequiresTarget => true;

        public Intern() : base() { }

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

using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Fabricator : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Fabricator() : base() { }

        protected override void Act()
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }
    }
}

using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Android : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Android() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Attack(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }
    }
}

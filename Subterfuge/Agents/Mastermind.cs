using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Mastermind : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Mastermind() : base() { }

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

using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Drudge : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Drudge() : base() { }

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

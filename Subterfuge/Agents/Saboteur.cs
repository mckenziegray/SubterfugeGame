using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Saboteur : Agent
    {
        public override Allegiance Allegiance => Allegiance.Enemy;
        public override bool RequiresTarget => true;

        public Saboteur() : base() { }

        protected override void Act()
        {
            if (Target.IsAlive && Target != this)
                Target.Block(this);
        }

        public override string GetReport()
        {
            throw new NotSupportedException();
        }
    }
}

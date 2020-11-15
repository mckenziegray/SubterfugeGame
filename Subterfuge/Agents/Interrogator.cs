using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Interrogator : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        public override bool RequiresTarget => true;

        public Interrogator() : base() { }

        protected override void Act()
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string GetReport()
        {
            string report;
            if (!Target.IsAlive || IsBlocked)
            {
                report = $"Due to unforeseen circumstances, I was unable to question {Target.Codename}. Don't worry, we'll get 'em next time.";
            }
            else
            {
                report = $"Per your orders, {Target.Codename} was interrogated.";
                // TODO: Get information
            }

            return report;
        }
    }
}

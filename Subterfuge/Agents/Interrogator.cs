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
            // Does nothing
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
                report = $"Per your orders, {Target.Codename} was interrogated";

                /*
                 * Scenarios:
                 * 1. The Fabricator tried to frame them
                 * 2. One of 2 or 3 possibilities
                 */

                if (Target.WasFramed)
                {
                    report += $", but something doesn't add up. I think someone tried to frame {Target.Gender.ToObjectivePronoun().ToLower()}.";
                }
                else
                {
                    report += Target switch
                    {
                        Android or Assassin or Sentry => $". {Target.Gender.ToCommonPronoun()} acted kinda robotic. {Target.Gender.ToCommonPronoun()} could be the Android, the Sentry, or the Assassin.",
                        Convoy or Drudge => $". {Target.Gender.ToCommonPronoun()} sure acts tough. I'd guess {Target.Gender.ToCommonPronoun().ToLower()}'s the Drudge or the Convoy.",
                        Cutout or Mastermind or Swallow => $". A real charmer, that one. Very friendly. Maybe even a little too friendly. {Target.Gender.ToCommonPronoun()}'s probably the Swallow/Raven, the Cut-out, or maybe the Mastermind.",
                        Fabricator or Hacker => $". {Target.Gender.ToCommonPronoun()} seems like kind of an egghead. {Target.Gender.ToCommonPronoun()}'s gotta be either the Hacker or the Fabricator.",
                        Marshal or Saboteur => $". What a kook. {Target.Gender.ToCommonPronoun()} must be eiher the Saboteur or that weird Marshal.",
                        Medic or Sleeper => $". {Target.Gender.ToCommonPronoun()}'s very soft-spoken, like that Medic. Or it could be the Sleeper.",
                        _ => throw new NotSupportedException()
                    };
                }

            }

            return report;
        }
    }
}

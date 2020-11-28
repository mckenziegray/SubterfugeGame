using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public class Interrogator : PlayerAgent
    {
        protected enum InterrogatorReportType
        {
            Android_Assassin_Sentry,
            Convoy_Drudge,
            Cutout_Mastermind_Swallow,
            Fabricator_Hacker,
            Marshal_Saboteur,
            Medic_Sleeper,
            Framed,
            Blocked,
            SelfIdentify
        }

        public override bool RequiresTarget => true;

        public Interrogator() : base() { }

        protected override void Act()
        {
            Target.Visit(this);
        }

        public override string GetReport()
        {
            InterrogatorReportType interrogatorReportType = GetInterrogatorReportType();
            string report;

            if (interrogatorReportType == InterrogatorReportType.Blocked)
            {
                report = $"Due to unforeseen circumstances, I was unable to question {Target.Codename}. Don't worry, we'll get 'em next time.";
            }
            else
            {
                report = $"Per your orders, {Target.Codename} was interrogated";

                if (interrogatorReportType == InterrogatorReportType.Framed)
                {
                    report += $", but something doesn't add up. I think someone tried to frame {Target.Gender.ToObjectivePronoun().ToLower()}.";
                }
                else
                {
                    report += interrogatorReportType switch
                    {
                        InterrogatorReportType.Android_Assassin_Sentry => $". {Target.Gender.ToCommonPronoun()} acted kinda robotic. {Target.Gender.ToCommonPronoun()} could be the Android, the Sentry, or the Assassin.",
                        InterrogatorReportType.Convoy_Drudge => $". {Target.Gender.ToCommonPronoun()} sure acts tough. I'd guess {Target.Gender.ToCommonPronoun().ToLower()}'s the Drudge or the Convoy.",
                        InterrogatorReportType.Cutout_Mastermind_Swallow => $". A real charmer, that one. Very friendly. Maybe even a little too friendly. {Target.Gender.ToCommonPronoun()}'s probably the Swallow/Raven, the Cut-out, or maybe the Mastermind.",
                        InterrogatorReportType.Fabricator_Hacker => $". {Target.Gender.ToCommonPronoun()} seems like kind of an egghead. {Target.Gender.ToCommonPronoun()}'s gotta be either the Hacker or the Fabricator.",
                        InterrogatorReportType.Marshal_Saboteur => $". What a kook. {Target.Gender.ToCommonPronoun()} must be eiher the Saboteur or that weird Marshal.",
                        InterrogatorReportType.Medic_Sleeper => $". {Target.Gender.ToCommonPronoun()}'s very soft-spoken, like that Medic. Or it could be the Sleeper.",
                        _ => throw new NotSupportedException()
                    };
                }
            }

            return report;
        }

        protected override string GetReportBriefAction()
        {
            InterrogatorReportType interrogatorReportType = GetInterrogatorReportType();
            return interrogatorReportType switch
            {
                InterrogatorReportType.Framed => $" {Target.Codename} was framed.",
                _ => $" {Target.Codename} is either" + interrogatorReportType switch
                {
                    InterrogatorReportType.Android_Assassin_Sentry => " the Android, the Assassin, or the Sentry.",
                    InterrogatorReportType.Convoy_Drudge => " the Convoy or the Drudge.",
                    InterrogatorReportType.Cutout_Mastermind_Swallow => " the Cut-out, the Mastermind, or the Swallow/Raven.",
                    InterrogatorReportType.Fabricator_Hacker => " the Fabricator or the Hacker.",
                    InterrogatorReportType.Marshal_Saboteur => " the Marshal or the Saboteur.",
                    InterrogatorReportType.Medic_Sleeper => " the Medic or the Sleeper.",
                    _ => throw new NotImplementedException()
                }
            };
        }

        protected override ReportType GetReportType()
        {
            return GetInterrogatorReportType() switch
            {
                InterrogatorReportType.Android_Assassin_Sentry or
                InterrogatorReportType.Convoy_Drudge or
                InterrogatorReportType.Cutout_Mastermind_Swallow or
                InterrogatorReportType.Fabricator_Hacker or
                InterrogatorReportType.Marshal_Saboteur or
                InterrogatorReportType.Medic_Sleeper or
                InterrogatorReportType.Framed => ReportType.Action,
                InterrogatorReportType.Blocked => ReportType.Blocked,
                InterrogatorReportType.SelfIdentify => ReportType.SelfIdentify,
                _ => throw new NotImplementedException(),
            };
        }

        protected InterrogatorReportType GetInterrogatorReportType()
        {
            if (!Target.IsActive || IsBlocked)
            {
                return InterrogatorReportType.Blocked;
            }
            else
            {
                return Target.WasFramed ? InterrogatorReportType.Framed : Target switch
                {
                    Android or Assassin or Sentry => InterrogatorReportType.Android_Assassin_Sentry,
                    Convoy or Drudge => InterrogatorReportType.Convoy_Drudge,
                    Cutout or Mastermind or Swallow => InterrogatorReportType.Cutout_Mastermind_Swallow,
                    Fabricator or Hacker => InterrogatorReportType.Fabricator_Hacker,
                    Marshal or Saboteur => InterrogatorReportType.Marshal_Saboteur,
                    Medic or Sleeper => InterrogatorReportType.Medic_Sleeper,
                    _ => throw new NotSupportedException()
                };
            }
        }
    }
}

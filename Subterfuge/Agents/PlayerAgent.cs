using System;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public abstract class PlayerAgent : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;
        /// <summary>
        /// Whether this agent deserted this round.
        /// </summary>
        public bool Deserted { get; set; }

        /// <summary>
        /// Removes this agent from the game.
        /// </summary>
        public void Desert()
        {
            IsActive = false;
            Deserted = true;
        }

        /// <summary>
        /// Gets this agent's full report for the round.
        /// </summary>
        /// <returns>A string containing the report.</returns>
        /// <remarks>
        /// Unit            Self-targets    Self-identifies
        /// Assassin        No              No
        /// Convoy          No              No
        /// Hacker          No              Yes
        /// Interrogator    No              Yes
        /// Marshal         No              Yes
        /// Medic           No              No
        /// Sentry          Yes             No
        /// Swallow/Raven   No              Yes
        /// </remarks>
        public abstract string GetReport();

        /// <summary>
        /// Gets a short version of this agent's report for the round.
        /// </summary>
        /// <returns>The brief.</returns>
        public string GetReportBrief()
        {
            return $"The {Name} reported that"
                + GetReportType() switch
                {
                    ReportType.Action => GetReportBriefAction(),
                    ReportType.Blocked => $" they were unable to act.",
                    ReportType.SelfIdentify => $" they are {Codename}.",
                    _ => throw new NotImplementedException()
                };
        }

        public override void Reset()
        {
            Deserted = false;
            base.Reset();
        }

        /// <summary>
        /// Gets the latter part of the short version of this agent's report for the round if they acted.
        /// </summary>
        /// <returns>The partial brief.</returns>
        /// <remarks>This method MUST be overridden by most children.</remarks>
        protected virtual string GetReportBriefAction()
        {
            string action = this switch
            {
                Assassin => $"killed",
                Marshal or Swallow => "role-blocked",
                Convoy or Medic or Hacker or Interrogator or Sentry => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };

            return $" {Target.Codename} was {action}.";
        }

        /// <summary>
        /// Gets the type of report to be returned by this agent this round.
        /// </summary>
        /// <returns>The report type.</returns>
        protected abstract ReportType GetReportType();
    }
}

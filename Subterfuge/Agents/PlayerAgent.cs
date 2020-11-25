using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subterfuge.Enums;

namespace Subterfuge.Agents
{
    public abstract class PlayerAgent : Agent
    {
        public override Allegiance Allegiance => Allegiance.Ally;

        /// <summary>
        /// Gets the unit's report. Only works for allied units.
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

        public string GetReportConcise()
        {
            return $"The {Name} reported that"
                + GetReportType() switch
                {
                    ReportType.Action => GetReportConciseAction(),
                    ReportType.Blocked => $" they were unable to act.",
                    ReportType.SelfIdentify => $" they are {Codename}.",
                    _ => throw new NotImplementedException()
                };
        }

        protected virtual string GetReportConciseAction()
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

        public abstract ReportType GetReportType();
    }
}

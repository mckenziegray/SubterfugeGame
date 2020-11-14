using System;
using System.Collections.Generic;
using System.Linq;
using DotNetExtensions;
using Subterfuge.Code;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class GameService
    {
        public const int MAX_AGENT_SELECTIONS = 2;

        private Random _random = new Random();
        public int Day { get; protected set; }
        public List<Agent> Agents { get; set; }
        public List<Agent> AgentsShuffled { get; set; }

        public GameService()
        {
            Day = 1;
            Agents = ((AgentType[])Enum.GetValues(typeof(AgentType))).Select(at => new Agent(at)).ToList();
            AgentsShuffled = new List<Agent>(Agents);
            AgentsShuffled.Shuffle(_random);
        }

        public void PlayRound()
        {
            foreach (Agent agent in Agents)
            {
                if (agent.IsActing)
                {
                    agent.Act();
                }
            }
        }

        public List<string> GetReports()
        {
            List<string> reports = new List<string>();

            foreach (Agent agent in Agents)
            {
                if (agent.IsActing)
                {
                    reports.Add($"{agent.AgentType}'s report: {agent.GetReport()}");
                }
            }

            return reports;
        }

        public void EndRound()
        {
            ++Day;
            //ReportMessages.Clear();
            Agents.ForEach(a => a.Reset());
        }
    }
}

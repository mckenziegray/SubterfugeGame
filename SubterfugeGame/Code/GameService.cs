using SubterfugeGame.Code;
using SubterfugeGame.Enums;
using SubterfugeGame.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubterfugeGame
{
    public class GameService
    {
        private Random random = new Random();
        public int Day { get; protected set; }
        public List<Agent> Agents { get; set; }
        public List<Agent> AgentsShuffled { get; set; }

        public GameService()
        {
            Day = 1;
            Agents = ((AgentType[])Enum.GetValues(typeof(AgentType))).Select(at => new Agent(at)).ToList();
            AgentsShuffled = new List<Agent>(Agents);
            AgentsShuffled.Shuffle(random);
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

            return reports
        }

        public void EndRound()
        {
            ++Day;
            ReportMessages.Clear();
            Agents.ForEach(a => a.Reset());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class GameService
    {
        public const int MAX_AGENT_SELECTIONS = 3;

        /// <summary>
        /// A list of all agent types. The order of this list is the order in which agents will act.
        /// </summary>
        /// <remarks>
        /// There should be NO DUPLICATES in this list.
        /// </remarks>
        public static readonly Type[] AGENT_TYPES_ORDERED =
        {
            typeof(Saboteur),
            typeof(Swallow),
            typeof(Marshal),
            typeof(Convoy),
            typeof(Medic),
            typeof(Android),
            typeof(Assassin),
            typeof(Drudge),
            typeof(Mastermind),
            typeof(Fabricator),
            typeof(Cutout),
            typeof(Interrogator),
            typeof(Hacker),
            typeof(Sentry),
            typeof(Sleeper)
        };

        public List<List<string>> Evidence { get; protected set; } = new();

        public static Random Random { get; private set; } = new();

        public int Round { get; protected set; }
        public AgentList Agents { get; protected set; }

        private static List<string> _generatedCodenames = new();

        public GameService()
        {
            Reset();
        }

        public void PlayRound()
        {
            foreach (Agent agent in Agents.OrderedList)
            {
                if (agent is NonPlayerAgent npc)
                    npc.SelectTarget(Agents);

                agent.ActIfAble();
            }

            #region Populate initial evidence
            List<string> roundEvidence = new List<string>();

            foreach (Agent agent in Agents.PlayerAgents.Where(a => a.IsActing))
            {
                string action = agent switch
                {
                    Assassin => $"kill",
                    Convoy or Medic => "protect",
                    Hacker or Interrogator => "investigate",
                    Marshal or Swallow => "block",
                    Sentry => "watch",
                    _ => throw new NotImplementedException()
                };
                roundEvidence.Add($"You ordered the {agent.Name} to {action} {agent.Target.Codename}.");
            }

            foreach (PlayerAgent agent in Agents.PlayerAgents.Where(a => a.IsActing && a.IsAlive))
                roundEvidence.Add(agent.GetReportConcise());

            foreach (Agent agent in Agents.OrderedList.Where(a => a.WasKilled))
                roundEvidence.Add($"The {agent.Killer.Name} killed the {agent.Name} ({agent.Codename}).");

            Evidence.Add(roundEvidence);
            #endregion
        }

        public void EndRound()
        {
            // This block is meant as a fail-safe and should never actually be hit.
            while (Evidence.Count < Round - 1)
                Evidence.Add(new List<string>());

            foreach (Agent agent in Agents.OrderedList.Where(a => a.WasExecuted))
                Evidence[Round - 1].Add($"You executed the {agent.Name} ({agent.Codename}).");
            
            ++Round;
            Agents.OrderedList.ForEach(a => a.Reset());
        }

        public void Reset()
        {
            Round = 1;
            Agents = new AgentList();
            Evidence.Clear();
            _generatedCodenames.Clear();
        }

        public static string GenerateUniqueCodename()
        {
            string codename;
            do
                codename = GenerateCodename();
            while (_generatedCodenames.Contains(codename));

            _generatedCodenames.Add(codename);

            return codename;
        }

        private static string GenerateCodename()
        {
            return string.Format("{0}{1}-{2}{3}{4}",
                (char)(Random.Next(26) + 'A'),
                (char)(Random.Next(26) + 'A'),
                Random.Next(10),
                Random.Next(10),
                Random.Next(10)
                );
        }
    }
}

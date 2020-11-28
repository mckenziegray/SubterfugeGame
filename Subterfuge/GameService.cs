using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class GameService
    {
        public const int MAX_AGENT_SELECTIONS = 3;
        public const double DESERTION_MORALE_THRESHOLD = -15;
        public const double CHANCE_TO_DESERT = 0.5;
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

        public static Random Random { get; private set; } = new();

        public int Round { get; protected set; }
        public double Morale { get; protected set; }
        public string MoraleLevel => Morale switch
        {
            <= -15 => "Panicked",
            <= -10 => "Alarmed",
            <= -5 => "Uneasy",
            _ => "Calm"
        };
        public AgentList Agents { get; protected set; }
        public ObservableCollection<List<string>> Evidence { get; protected set; } = new();

        private static List<string> _generatedCodenames = new();

        public GameService()
        {
            Reset();
        }

        /// <summary>
        /// Generates an random codename that no other agent in this game has yet.
        /// </summary>
        /// <returns>The generated codename.</returns>
        public static string GenerateUniqueCodename()
        {
            string codename;
            do
                codename = GenerateCodename();
            while (_generatedCodenames.Contains(codename));

            _generatedCodenames.Add(codename);

            return codename;
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void Reset()
        {
            Round = 1;
            Morale = 0;
            Agents = new AgentList();
            Evidence.Clear();
            _generatedCodenames.Clear();
        }

        /// <summary>
        /// Runs all automated functions for the round. This includes NPC target selection, agent actions, and storing all related evidence.
        /// </summary>
        public void PlayRound()
        {
            foreach (Agent agent in Agents.OrderedList)
            {
                if (agent is NonPlayerAgent npc)
                    npc.SelectTarget(Agents);

                agent.ActIfAble();
            }

            #region Populate initial evidence
            while (Evidence.Count < Round)
                Evidence.Add(new List<string>());

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
                Evidence[Round - 1].Add($"You ordered the {agent.Name} to {action} {agent.Target.Codename}.");
            }

            foreach (PlayerAgent agent in Agents.PlayerAgents.Where(a => a.IsActing && a.IsActive))
                Evidence[Round - 1].Add(agent.GetReportBrief());

            foreach (Agent agent in Agents.OrderedList.Where(a => a.WasKilled))
                Evidence[Round - 1].Add($"The {agent.Killer.Name} killed the {agent.Name} ({agent.Codename}).");
            #endregion
        }

        /// <summary>
        /// Ends the round and adds execution information to evidence if necessary.
        /// </summary>
        public void EndRound()
        {
            // This block is meant as a fail-safe and should never actually be hit.
            while (Evidence.Count < Round)
                Evidence.Add(new List<string>());

            foreach (Agent agent in Agents.OrderedList)
            {
                if (agent.WasExecuted)
                {
                    Evidence[Round - 1].Add($"You executed the {agent.Name} ({agent.Codename}).");
                    if (agent is Sleeper && agent.Target is not null && agent.Target.WasKilled)
                        Evidence[Round - 1].Add($"The {agent.Name} killed the {agent.Target.Name} ({agent.Target.Codename}).");

                    if (agent is Cutout)
                        Morale -= 10;
                    else if (agent.Allegiance == Allegiance.Ally)
                        Morale -= 5;
                    else if (agent.Allegiance == Allegiance.Enemy)
                        Morale += 3;
                }
                else if (agent.WasKilled)
                {
                    if (agent.Allegiance == Allegiance.Enemy)
                        Morale += 2;
                    else
                        Morale -= 3;
                }
            }
            
            ++Round;
            Agents.OrderedList.ForEach(a => a.Reset());

            // Determine if an agent will desert
            // this block mus come after the agents have reset
            if (Morale < DESERTION_MORALE_THRESHOLD)
            {
                if (Random.NextDouble() < CHANCE_TO_DESERT)
                {
                    List<Agent> possibleDeserters = Agents.ShuffledList.Where(a => a is PlayerAgent && a.IsActive).ToList();
                    PlayerAgent deserter = (PlayerAgent)possibleDeserters[Random.Next(possibleDeserters.Count)];
                    deserter.Desert();

                    while (Evidence.Count < Round)
                        Evidence.Add(new List<string>());

                    Evidence[Round - 1].Add($"The {deserter.Name} deserted.");
                }
            }
        }

        /// <summary>
        /// Generates a random agent codename.
        /// </summary>
        /// <returns>The generated codename.</returns>
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

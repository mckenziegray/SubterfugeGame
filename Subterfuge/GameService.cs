﻿using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge
{
    public class GameService
    {
        public const int MAX_AGENT_SELECTIONS = 3;

        public static Random Random { get; private set; } = new Random();
        public int Day { get; protected set; }
        public AgentList Agents { get; set; }

        private static readonly List<string> _generatedCodenames = new List<string>();

        public GameService()
        {
            Reset();
        }

        public void PlayRound()
        {
            foreach (Agent agent in Agents.OrderedList)
            {
                if (agent.Allegiance != Allegiance.Ally)
                    agent.SelectTarget(Agents);

                agent.ActIfAble();
            }
        }

        public void EndRound()
        {
            ++Day;
            Agents.OrderedList.ForEach(a => a.Reset());
        }

        public void Reset()
        {
            Day = 1;
            Agents = new AgentList();
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

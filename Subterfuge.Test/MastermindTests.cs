using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class MastermindTests
    {
        protected GameService Game { get; set; }
        protected Mastermind Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Mastermind", Agent.Name);
        }

        [Test]
        public void TestAllegiance()
        {
            Assert.AreEqual(Allegiance.Enemy, Agent.Allegiance);
        }

        [Test]
        public void TestRequiresTarget()
        {
            Assert.AreEqual(true, Agent.RequiresTarget);
        }

        [Test]
        public void TestSelectTarget()
        {
            Type[] invalidTargets = GameService.AGENT_TYPES_ORDERED.Where(t => Game.Agents[t.Name].Allegiance != Allegiance.Ally).ToArray();
            int numTargets = 10000;
            List<Type> targets = new List<Type>(numTargets);

            // Drudge is still alive, so no target should be selected
            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                Assert.IsNull(Agent.Target);
            }

            // Once Drudge is dead, targets can be selected
            Game.Agents[nameof(Drudge)].Execute();
            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                if (Agent.Target is not null)
                {
                    Assert.IsFalse(invalidTargets.Contains(Agent.Target.GetType()));
                    targets.Add(Agent.Target.GetType());
                }
                Agent.Reset();
            }

            // With enough trials, every valid target should have been selected at least once
            Assert.AreEqual(Game.Agents.OrderedList.Where(a => !invalidTargets.Contains(a.GetType())).Count(), targets.Distinct().Count());
        }

        [Test]
        public void TestAct()
        {
            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Marshal)], Agent, Game, false, false);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];

            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Swallow)], Agent, Game, false, false);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];

            Game.Agents[nameof(Drudge)].Execute();
            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Marshal)], Agent, Game, true, true);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];

            Game.Agents[nameof(Drudge)].Execute();
            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Swallow)], Agent, Game, true, true);
            Game.Reset();
            Agent = (Mastermind)Game.Agents[nameof(Mastermind)];
        }
    }
}

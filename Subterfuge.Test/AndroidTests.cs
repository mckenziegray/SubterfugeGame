using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class AndroidTests
    {
        protected GameService Game { get; set; }
        protected Android Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Android)Game.Agents[nameof(Android)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Android", Agent.Name);
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
            Type[] invalidTargets = { typeof(Android), typeof(Mastermind) };
            int numTargets = 10000;
            List<Type> targets = new List<Type>(numTargets);

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
            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Marshal)], Agent, Game, true, true);
            Game.Reset();
            Agent = (Android)Game.Agents[nameof(Android)];

            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Swallow)], Agent, Game, true, true);
            Game.Reset();
            Agent = (Android)Game.Agents[nameof(Android)];

            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Marshal)], Agent, Game, true, false);
            Game.Reset();
            Agent = (Android)Game.Agents[nameof(Android)];

            Helpers.TestBlockerKillInteraction(Game.Agents[nameof(Swallow)], Agent, Game, true, false);
            Game.Reset();
            Agent = (Android)Game.Agents[nameof(Android)];
        }
    }
}

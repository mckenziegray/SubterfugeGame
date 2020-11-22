using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class DrudgeTests
    {
        protected GameService Game { get; set; }
        protected Drudge Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Drudge)Game.Agents[nameof(Drudge)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Drudge", Agent.Name);
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

            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                Assert.IsNotNull(Agent.Target);
                Assert.IsFalse(invalidTargets.Contains(Agent.Target.GetType()));
                targets.Add(Agent.Target.GetType());
                Agent.Reset();
            }

            // With enough trials, every valid target should have been selected at least once
            Assert.AreEqual(Game.Agents.OrderedList.Where(a => !invalidTargets.Contains(a.GetType())).Count(), targets.Distinct().Count());
        }

        [Test]
        public void TestAct()
        {
            Helpers.TestKillAction(Agent, Game);
        }
    }
}

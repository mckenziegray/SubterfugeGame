using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class FabricatorTests
    {
        protected GameService Game { get; set; }
        protected Fabricator Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Fabricator)Game.Agents[nameof(Fabricator)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Fabricator", Agent.Name);
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
            Type[] invalidTargets = GameService.AGENT_TYPES_ORDERED.Where(t => Game.Agents[t.Name].Allegiance == Allegiance.Enemy).ToArray();
            int numTargets = 10000;
            List<Type> targets = new List<Type>(numTargets);

            Agent mastermind = Game.Agents[nameof(Mastermind)];
            Agent drudge = Game.Agents[nameof(Drudge)];

            for (int i = 0; i < numTargets; i++)
            {
                mastermind.SelectTarget(Game.Agents);
                drudge.SelectTarget(Game.Agents);
                Agent.SelectTarget(Game.Agents);
                Assert.IsNotNull(Agent.Target);
                Assert.IsFalse(invalidTargets.Contains(Agent.Target.GetType()));
                Assert.AreNotSame(mastermind.Target, Agent.Target);
                Assert.AreNotSame(drudge.Target, Agent.Target);
                targets.Add(Agent.Target.GetType());
                Agent.Reset();
                mastermind.Reset();
                drudge.Reset();
            }

            // With enough trials, every valid target should have been selected at least once
            Assert.AreEqual(Game.Agents.OrderedList.Where(a => !invalidTargets.Contains(a.GetType())).Count(), targets.Distinct().Count());
        }

        [Test]
        public void TestAct()
        {
            Agent.SelectTarget(Game.Agents);
            Agent.ActIfAble();
            Assert.IsTrue(Agent.Target.WasFramed);
            Agent.Target.Reset();
            Agent.Reset();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class CutoutTests
    {
        protected GameService Game { get; set; }
        protected Cutout Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Cutout)Game.Agents[nameof(Cutout)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Cut-out", Agent.Name);
        }

        [Test]
        public void TestAllegiance()
        {
            Assert.AreEqual(Allegiance.Neutral, Agent.Allegiance);
        }

        [Test]
        public void TestRequiresTarget()
        {
            Assert.AreEqual(true, Agent.RequiresTarget);
        }

        [Test]
        public void TestSelectTarget()
        {
            Type[] invalidTargets = { typeof(Cutout) };
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
            Agent.IsActing = true;
            Agent.SelectTarget(Game.Agents);
            Agent.ActIfAble();
            Assert.IsTrue(Agent.Target.Visitors.Contains(Agent.Codename));
        }
    }
}

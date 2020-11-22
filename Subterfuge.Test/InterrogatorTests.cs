using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class InterrogatorTests
    {
        protected GameService Game { get; set; }
        protected Interrogator Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Interrogator)Game.Agents[nameof(Interrogator)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Interrogator", Agent.Name);
        }

        [Test]
        public void TestAllegiance()
        {
            Assert.AreEqual(Allegiance.Ally, Agent.Allegiance);
        }

        [Test]
        public void TestRequiresTarget()
        {
            Assert.AreEqual(true, Agent.RequiresTarget);
        }

        [Test]
        public void TestSelectTarget()
        {
            Assert.Throws<NotSupportedException>(() => Agent.SelectTarget(Game.Agents));
        }
    }
}

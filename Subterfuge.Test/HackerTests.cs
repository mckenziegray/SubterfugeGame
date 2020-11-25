using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class HackerTests
    {
        protected GameService Game { get; set; }
        protected Hacker Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Hacker)Game.Agents[nameof(Hacker)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Hacker", Agent.Name);
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
    }
}

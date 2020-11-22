using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class ConvoyTests
    {
        protected GameService Game { get; set; }
        protected Convoy Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Convoy)Game.Agents[nameof(Convoy)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Convoy", Agent.Name);
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

        [Test]
        public void TestAct()
        {
            Helpers.TestProtectAction(Agent, Game, true);
        }
    }
}

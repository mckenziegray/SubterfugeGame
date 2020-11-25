using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class AssassinTests
    {
        protected GameService Game { get; set; }
        protected Assassin Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Assassin)Game.Agents[nameof(Assassin)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Assassin", Agent.Name);
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
        public void TestAct()
        {
            Helpers.TestKillAction(Agent, Game);
        }
    }
}

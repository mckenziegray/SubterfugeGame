using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class MarshalTests
    {
        protected GameService Game { get; set; }
        protected Marshal Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Marshal)Game.Agents[nameof(Marshal)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Marshal", Agent.Name);
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
            Helpers.TestBlockAction(Agent, Game);
            Helpers.TestProtectAction(Agent, Game, false);
            Game.Reset();
            Agent = (Marshal)Game.Agents[nameof(Marshal)];

            Helpers.TestBlockerKillInteraction(Agent, Game.Agents[nameof(Android)], Game, true, true);
            Game.Reset();
            Agent = (Marshal)Game.Agents[nameof(Marshal)];

            Helpers.TestBlockerKillInteraction(Agent, Game.Agents[nameof(Mastermind)], Game, true, true);
            Game.Reset();
            Agent = (Marshal)Game.Agents[nameof(Marshal)];
        }
    }
}

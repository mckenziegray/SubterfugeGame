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
        public void TestSelectTarget()
        {
            Assert.Throws<NotSupportedException>(() => Agent.SelectTarget(Game.Agents));
        }

        [Test]
        public void TestAct()
        {
            Helpers.TestBlockAction(Agent, Game);
            Helpers.TestProtectAction(Agent, Game, false);

            Agent android = Game.Agents[nameof(Android)];
            android.IsActing = true;
            android.Target = Game.Agents[nameof(Hacker)];
            Agent.IsActing = true;
            Agent.Target = android;

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                Game.Agents[agentType.Name].ActIfAble();
            }

            Assert.IsFalse(android.IsBlocked);
            Assert.IsNull(android.Blocker);
            Assert.AreSame(Agent, android.Target);
            Assert.IsTrue(Agent.WasKilled);
            Assert.AreSame(android, Agent.Killer);
        }
    }
}

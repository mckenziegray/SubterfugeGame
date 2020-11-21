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
            Agent target = Game.Agents[nameof(Hacker)];
            Agent attacker = Game.Agents[nameof(Android)];

            Agent.IsActing = true;
            Agent.Target = target;
            attacker.IsActing = true;
            attacker.Target = target;

            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
            {
                Game.Agents[agentType.Name].ActIfAble();
            }

            Assert.IsTrue(target.WasAttacked);
            Assert.IsFalse(target.WasKilled);
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(target.IsProtected);
            Assert.AreSame(target.Protector, Agent);

            Assert.IsFalse(Agent.WasAttacked);
            Assert.IsTrue(Agent.WasKilled);
            Assert.IsFalse(Agent.IsAlive);
            Assert.AreSame(Agent.Killer, attacker);

            Assert.IsFalse(attacker.WasAttacked);
            Assert.IsTrue(attacker.WasKilled);
            Assert.IsFalse(attacker.IsAlive);
            Assert.AreSame(attacker.Killer, Agent);
        }
    }
}

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
            Agent = (Assassin)Game.Agents[Agent.GetType().Name];
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
        public void TestSelectTarget()
        {
            Assert.Throws<NotSupportedException>(() => Agent.SelectTarget(Game.Agents));
        }

        [Test]
        public void TestAct()
        {
            Agent target = Game.Agents[nameof(Hacker)];
            Agent protector = Game.Agents[nameof(Medic)];

            Agent.Target = target;
            Agent.IsActing = true;

            target.Protect(protector);
            Agent.ActIfAble();
            Assert.IsTrue(target.WasAttacked);
            Assert.IsTrue(target.IsAlive);
            Agent.Reset();
            target.Reset();

            Agent.ActIfAble();
            Assert.IsTrue(target.WasAttacked);
            Assert.IsFalse(target.IsAlive);
            Game.Reset();
            Agent = (Assassin)Game.Agents[nameof(Assassin)];
        }
    }
}

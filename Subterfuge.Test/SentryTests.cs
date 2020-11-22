using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class SentryTests
    {
        protected GameService Game { get; set; }
        protected Sentry Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Sentry)Game.Agents[nameof(Sentry)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Sentry", Agent.Name);
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

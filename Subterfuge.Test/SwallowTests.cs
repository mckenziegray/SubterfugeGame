using System;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class SwallowTests
    {
        protected GameService Game { get; set; }
        protected Swallow Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Swallow)Game.Agents[nameof(Swallow)];
        }

        [Test]
        public void TestName()
        {
            for (int i = 0; i < 1000; i++)
            {
                Agent = new Swallow();
                switch (Agent.Gender)
                {
                    case Gender.Female:
                        Assert.AreEqual("Swallow", Agent.Name);
                        break;
                    case Gender.Male:
                        Assert.AreEqual("Raven", Agent.Name);
                        break;
                    default:
                        throw new NotImplementedException();
                };
            }

            Agent = (Swallow)Game.Agents[nameof(Swallow)];
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
            Game.Reset();
            Agent = (Swallow)Game.Agents[nameof(Swallow)];

            Helpers.TestBlockerKillInteraction(Agent, Game.Agents[nameof(Android)], Game, true, true);
            Game.Reset();
            Agent = (Swallow)Game.Agents[nameof(Swallow)];

            Helpers.TestBlockerKillInteraction(Agent, Game.Agents[nameof(Mastermind)], Game, true, true);
            Game.Reset();
            Agent = (Swallow)Game.Agents[nameof(Swallow)];
        }
    }
}

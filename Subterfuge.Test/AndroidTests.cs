using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge.Test
{
    public class AndroidTests
    {
        protected GameService Game { get; set; }
        protected Android Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            Game = new();
            Agent = (Android)Game.Agents[nameof(Android)];
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Android", Agent.Name);
        }

        [Test]
        public void TestAllegiance()
        {
            Assert.AreEqual(Allegiance.Enemy, Agent.Allegiance);
        }

        [Test]
        public void TestRequiresTarget()
        {
            Assert.AreEqual(true, Agent.RequiresTarget);
        }

        [Test]
        public void TestSelectTarget()
        {
            Type[] invalidTargets = { typeof(Android), typeof(Mastermind) };
            int numTargets = 10000;
            List<Type> targets = new List<Type>(numTargets);

            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                if (Agent.Target is not null)
                {
                    Assert.IsFalse(invalidTargets.Contains(Agent.Target.GetType()));
                    targets.Add(Agent.Target.GetType());
                }
                Agent.Reset();
            }

            // With enough trials, every valid target should have been selected at least once
            Assert.AreEqual(Game.Agents.OrderedList.Where(a => !invalidTargets.Contains(a.GetType())).Count(), targets.Distinct().Count());
        }

        [Test]
        public void TestAct()
        {
            Agent target = Game.Agents[nameof(Hacker)];

            Helpers.TestKillAction(Agent, Game);
            Game.Reset();
            Agent = (Android)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            // If targeted by a blocker, the Android should change targets and kill the blocker (without visiting either one)
            Agent blocker1 = Game.Agents[nameof(Swallow)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker1.IsActing = true;
            blocker1.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            //Assert.IsTrue(Agent.Target == blocker1);
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker1);
            Assert.IsTrue(blocker1.WasAttacked);
            //Assert.IsFalse(blocker1.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker1.IsAlive);
            Game.Reset();
            Agent = (Android)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];

            Agent blocker2 = Game.Agents[nameof(Marshal)];
            Agent.IsActing = true;
            Agent.Target = target;
            blocker2.IsActing = true;
            blocker2.Target = Agent;
            foreach (Type agentType in GameService.AGENT_TYPES_ORDERED)
                Game.Agents[agentType.Name].ActIfAble();
            Assert.IsTrue(Agent.Target == blocker2);
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker2);
            Assert.IsTrue(blocker2.WasAttacked);
            //Assert.IsFalse(blocker2.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker2.IsAlive);
            Game.Reset();
            Agent = (Android)Game.Agents[Agent.GetType().Name];
            target = Game.Agents[nameof(Hacker)];
        }
    }
}

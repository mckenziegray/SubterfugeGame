using System;
using System.Collections.Generic;
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
            Agent = (Android)Game.Agents[Agent.GetType().Name];
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
            int numTargets = 1000;
            List<Type> targets = new List<Type>(numTargets);

            for (int i = 0; i < numTargets; i++)
            {
                Agent.SelectTarget(Game.Agents);
                if (Agent.Target is not null)
                {
                    Assert.IsTrue(Agent.Target.GetType() != typeof(Android) && Agent.Target.GetType() != typeof(Mastermind));
                    targets.Add(Agent.Target.GetType());
                }
                Agent.Reset();
            }

            Assert.AreNotEqual(0, targets.Count);
        }

        [Test]
        public void TestAct()
        {
            Agent target = Game.Agents[nameof(Hacker)];
            Agent protector = Game.Agents[nameof(Medic)];
            Agent blocker1 = Game.Agents[nameof(Swallow)];
            Agent blocker2 = Game.Agents[nameof(Marshal)];

            // A protected target should be visited and attacked but not killed
            Agent.IsActing = true;
            Agent.Target = target;
            Agent.Target.Protect(protector);
            Agent.ActIfAble();
            Assert.IsTrue(Agent.Target.WasAttacked);
            Assert.IsTrue(Agent.Target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(Agent.Target.IsAlive);
            Agent.Reset();
            Agent.Target.Reset();

            // An unprotected target should be visited and killed
            Agent.IsActing = true;
            Agent.Target = target;
            Agent.ActIfAble();
            Assert.IsTrue(Agent.Target.WasAttacked);
            Assert.IsTrue(Agent.Target.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(Agent.Target.IsAlive);
            Agent.Reset();
            Agent.Target.Reset();

            // If targeted by a blocker, the Android should change targets and kill the blocker (without visiting either one)
            Agent.IsActing = true;
            Agent.Target = target;
            blocker1.IsActing = true;
            blocker1.Target = Agent;
            blocker1.Block(Agent);
            Agent.ActIfAble();
            Assert.IsFalse(Agent.Target == target);
            Assert.IsFalse(target.WasAttacked);
            Assert.IsFalse(target.Visitors.Contains(Agent.Codename));
            Assert.IsTrue(target.IsAlive);
            Assert.IsTrue(Agent.Target == blocker1);
            Assert.IsTrue(blocker1.WasAttacked);
            Assert.IsFalse(blocker1.Visitors.Contains(Agent.Codename));
            Assert.IsFalse(blocker1.IsAlive);
            Game.Reset();
            Agent = (Assassin)Game.Agents[Agent.GetType().Name];
        }
    }
}
